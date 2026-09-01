using System.Reflection.Emit;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;

namespace AjamaGhouligan.AjamaGhouliganCode.Patches;

// From @halfocused in #sts2-modding, dev of Spire Enigmas

[HarmonyPatch(typeof(NHandCardHolder), nameof(NHandCardHolder.UpdateCard))]
public static class SpectreFormGlowPatch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codeMatcher = new CodeMatcher(instructions, generator);

        codeMatcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0), //this
            new CodeMatch(OpCodes.Call), //get_CardNode
            new CodeMatch(OpCodes.Callvirt), //get_Model
            new CodeMatch(OpCodes.Callvirt), //CanPlay()
            new CodeMatch(OpCodes.Brtrue_S)

        ).ThrowIfInvalid("Could not find card model check");

        object getCardNodeMethod = codeMatcher.InstructionAt(1).operand;
        object getModelMethod = codeMatcher.InstructionAt(2).operand;

        //we advance late for the sake of setting those variables
        codeMatcher.Advance(5);

        codeMatcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Call),
            new CodeMatch(OpCodes.Callvirt),
            new CodeMatch(OpCodes.Callvirt),

            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Call),
            new CodeMatch(OpCodes.Callvirt),
            new CodeMatch(OpCodes.Ldsfld),
            new CodeMatch(OpCodes.Callvirt) //set_Modulate
        ).ThrowIfInvalid("Could not find glow color assignment");

        codeMatcher.Advance(8);

        codeMatcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0), //this
            new CodeInstruction(OpCodes.Call, getCardNodeMethod),
            new CodeInstruction(OpCodes.Callvirt, getModelMethod),
            #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            CodeInstruction.Call(() => PossiblyTweakColor(default, default))
            #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        );

        return codeMatcher.Instructions();
    }

    static Color PossiblyTweakColor(Color oldColor, CardModel cardModel)
    {
        var spectreFormPowers = cardModel.Owner.Creature.Powers.Where(p => p is SpectreFormPower);

        if (spectreFormPowers.Any(p => p.GetInternalData<SpectreFormPower.Data>().CardsLeft == 1))
        {
            return new Color("ff79f0");
        }

        return oldColor;
    }
}