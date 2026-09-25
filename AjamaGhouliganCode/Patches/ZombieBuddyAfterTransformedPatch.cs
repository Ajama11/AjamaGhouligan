using System.Reflection;
using System.Reflection.Emit;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Rare.Attack;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Random;

namespace AjamaGhouligan.AjamaGhouliganCode.Patches;

[HarmonyPatch]
public static class ZombieBuddyAfterTransformedPatch
{
    [HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Transform),
        typeof(IEnumerable<CardTransformation>),
        typeof(Rng),
        typeof(CardPreviewStyle))]
    [HarmonyPatch(MethodType.Async)]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        MethodInfo updateZombieBuddies = typeof(ZombieBuddyAfterTransformedPatch).Method(nameof(UpdateZombieBuddies));
        
        CodeMatcher matcher = new CodeMatcher(instructions)
            .MatchStartForward([
                new CodeMatch(OpCodes.Ldfld),   // 0: original
                CodeMatch.Calls(typeof(CardPile).Method(nameof(CardModel.AfterTransformedFrom))), // 1
                new CodeMatch(OpCodes.Ldarg_0), // 2
                new CodeMatch(OpCodes.Ldfld),   // 3: replacement
                CodeMatch.Calls(typeof(CardPile).Method(nameof(CardModel.AfterTransformedTo))), // 4
            ])
            .ThrowIfInvalid("ZombieBuddyAfterTransformedPatch could not find the correct position");

        var original = matcher.InstructionAt(0).operand;
        var replacement = matcher.InstructionAt(3).operand;

        matcher.Advance(4) // After AfterTransformedTo()
            .Insert([
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, replacement),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, original),
                new CodeInstruction(OpCodes.Call, updateZombieBuddies)
            ]);
        
        return matcher.InstructionEnumeration().ToList();
    }
    
    private static void UpdateZombieBuddies(CardModel original, CardModel replacement)
    {
        if (replacement.Owner?.PlayerCombatState == null) return;

        foreach (var zombie in replacement.Owner.PlayerCombatState.AllCards.OfType<ZombieBuddy>())
        {
            if (zombie.Card == original)
            {
                zombie.SetCardToTransform();
                MainFile.Logger.Warn($"A Zombie's target {original.Title} got transformed, so it is now targeting {zombie.Card?.Title}");
            }
        }
    }
}