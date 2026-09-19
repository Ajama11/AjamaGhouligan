using System.Reflection;
using System.Reflection.Emit;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Patches;

[HarmonyPatch(typeof(PowerCmd))]
public static class PattyCakeSpeedUpPatch
{
    [HarmonyPatch(nameof(PowerCmd.Apply), MethodType.Async)]
    [HarmonyPatch([
        typeof(PlayerChoiceContext),
        typeof(PowerModel),
        typeof(Creature),
        typeof(decimal),
        typeof(Creature),
        typeof(CardModel),
        typeof(bool)
    ])]
    [HarmonyTranspiler]
    private static List<CodeInstruction> ApplyPowerTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        MethodInfo replaceDuration = typeof(PattyCakeSpeedUpPatch).Method(nameof(ReplaceDuration));
        
        CodeMatcher matcher = new CodeMatcher(instructions)
            .MatchStartForward([
                new CodeMatch(OpCodes.Ldarg_0),   // 0: this
                new CodeMatch(OpCodes.Ldfld),     // 1: Load power
                new CodeMatch(OpCodes.Callvirt),  // 2: get_IsVisible()
                new CodeMatch(OpCodes.Brfalse_S), // 3
                new CodeMatch(OpCodes.Call),      // 4: get_Instance()
                new CodeMatch(OpCodes.Callvirt),  // 5: get_IsInProgress()
                new CodeMatch(OpCodes.Brfalse_S), // 6
                new CodeMatch(OpCodes.Ldc_R4),    // 7: Load 0.1f
                new CodeMatch(OpCodes.Ldc_R4),    // 8: Load 0.25f
            ])
            .ThrowIfInvalid("PattyCakeSpeedUpPatch ApplyPowerTranspiler could not find the correct position");
        
        var powerParameter = matcher.InstructionAt(1).operand;

        matcher.Advance(8) // In between 0.1f and 0.25f
            .Insert([
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, powerParameter),
                new CodeInstruction(OpCodes.Call, replaceDuration)
            ]);
        
        return matcher.InstructionEnumeration().ToList();
    }
    
    [HarmonyPatch(nameof(PowerCmd.Remove), MethodType.Async)]
    [HarmonyPatch([typeof(PowerModel)])]
    [HarmonyTranspiler]
    private static List<CodeInstruction> RemovePowerTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        MethodInfo replaceDuration = typeof(PattyCakeSpeedUpPatch).Method(nameof(ReplaceDuration));
        
        CodeMatcher matcher = new CodeMatcher(instructions)
            .MatchStartForward([
                new CodeMatch(OpCodes.Ldarg_0),  // 0: this
                new CodeMatch(OpCodes.Ldfld),    // 1: Load power
                new CodeMatch(OpCodes.Callvirt), // 2: RemoveInternal()
                new CodeMatch(OpCodes.Ldc_R4),   // 3: Load 0.2f
                new CodeMatch(OpCodes.Ldc_R4),   // 4: Load 0.4f
            ])
            .ThrowIfInvalid("PattyCakeSpeedUpPatch RemovePowerTranspiler could not find the correct position");
        
        var powerParameter = matcher.InstructionAt(1).operand;

        matcher.Advance(4) // In between 0.2f and 0.4f
            .Insert([
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, powerParameter),
                new CodeInstruction(OpCodes.Call, replaceDuration)
            ]);
        
        return matcher.InstructionEnumeration().ToList();
    }

    private static float ReplaceDuration(float original, PowerModel power)
    {
        return power is PattyCakePower ? 0.01f : original;
    }
}