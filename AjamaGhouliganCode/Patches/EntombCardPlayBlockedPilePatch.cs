using System.Reflection;
using System.Reflection.Emit;
using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.MoveToResultPileWithoutPlaying), MethodType.Async)]
public static class EntombCardPlayBlockedPilePatch
{
    [HarmonyTranspiler]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        MethodInfo replacePileType = typeof(EntombCardPlayBlockedPilePatch).Method(nameof(ReplacePileType));
        
        CodeMatcher matcher = new CodeMatcher(instructions)
            .MatchStartForward([
                new CodeMatch(OpCodes.Ldloc_1),  // 0: Load card (this)
                new CodeMatch(OpCodes.Ldc_I4_3), // 1: Load 3 (Discard)
                new CodeMatch(OpCodes.Ldc_I4_1), // 2: Load 1 (Bottom)
                new CodeMatch(OpCodes.Ldnull),   // 3: Load null
                new CodeMatch(OpCodes.Ldc_I4_0), // 4: Load 0 (false)
                new CodeMatch(OpCodes.Call)      // 5: CardPileCmd.Add()
            ])
            .ThrowIfInvalid("EntombCardPlayBlockedPilePatch could not find the correct position");

        matcher.Advance(2) // In between 3 (Discard) and 1 (Bottom)
            .Insert([
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, replacePileType)
            ]);
        
        return matcher.InstructionEnumeration().ToList();
    }

    private static int ReplacePileType(int original, CardModel card)
    {
        return card.Keywords.Contains(MyEnums.Entomb) ? (int) SepulchrePile.PileType : original;
    }
}