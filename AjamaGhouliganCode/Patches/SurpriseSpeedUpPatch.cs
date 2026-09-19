using System.Reflection;
using System.Reflection.Emit;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper), MethodType.Async)]
public static class SurpriseSpeedUpPatch
{
    [HarmonyTranspiler]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        MethodInfo replaceDuration = typeof(SurpriseSpeedUpPatch).Method(nameof(ReplaceDuration));
        
        CodeMatcher matcher = new CodeMatcher(instructions)
            .MatchStartForward([
                new CodeMatch(OpCodes.Pop),     // 0: These are just here
                new CodeMatch(OpCodes.Ldarg_0), // 1: to make sure I grab
                new CodeMatch(OpCodes.Ldfld),   // 2: the CustomScaledWait
                new CodeMatch(OpCodes.Brtrue),  // 3: in isAutoPlay
                new CodeMatch(OpCodes.Ldc_R4),  // 4: Load 0.25f
                new CodeMatch(OpCodes.Ldc_R4)   // 5: Load 0.35f
            ])
            .ThrowIfInvalid("SurpriseSpeedUpPatch could not find the correct position");

        matcher.Advance(5) // In between 0.25f and 0.35f
            .Insert([
                new CodeInstruction(OpCodes.Ldloc_1), // Load cardModel
                new CodeInstruction(OpCodes.Call, replaceDuration)
            ]);
        
        return matcher.InstructionEnumeration().ToList();
    }
    
    private static float ReplaceDuration(float original, CardModel card)
    {
        return card is Surprise ? 0.01f : original;
    }
}