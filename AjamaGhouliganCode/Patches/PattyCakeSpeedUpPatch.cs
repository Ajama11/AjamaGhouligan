using AjamaGhouligan.AjamaGhouliganCode.Powers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace AjamaGhouligan.AjamaGhouliganCode.Patches;

[HarmonyPatch]
public static class PattyCakeSpeedUpPatch
{
    [HarmonyPatch(typeof(PowerCmd), nameof(PowerCmd.Apply), 
        typeof(PlayerChoiceContext),
        typeof(PowerModel),
        typeof(Creature),
        typeof(decimal),
        typeof(Creature),
        typeof(CardModel),
        typeof(bool))]
    [HarmonyPrefix]
    private static void ApplyPrefix(ref PowerModel power)
    {
        if (power is not PattyCakePower pattyCake) return;

        pattyCake.IsInPowerCmdApply = true;
        
        power = pattyCake;
    }

    [HarmonyPatch(typeof(NPowerContainer), nameof(NPowerContainer.Add))]
    [HarmonyPrefix]
    private static bool AddNPowerPrefix(NPowerContainer __instance, PowerModel power)
    {
        if (power is not PattyCakePower) return true;

        NPower nPower = NPower.Create(power);
        nPower.Container = __instance;
        __instance._powerNodes.Add(nPower);
        __instance.AddChildSafely(nPower);
        __instance.UpdatePositions();
        
        return false;
    }
    
    [HarmonyPatch(typeof(PowerCmd), nameof(PowerCmd.Apply), 
        typeof(PlayerChoiceContext),
        typeof(PowerModel),
        typeof(Creature),
        typeof(decimal),
        typeof(Creature),
        typeof(CardModel),
        typeof(bool))]
    [HarmonyPostfix]
    private static void ApplyPostfix(ref PowerModel power)
    {
        if (power is not PattyCakePower pattyCake) return;
        
        pattyCake.IsInPowerCmdApply = false;
        
        power = pattyCake;
    }

    [HarmonyPatch(typeof(PowerCmd), nameof(PowerCmd.Remove), typeof(PowerModel))]
    [HarmonyPrefix]
    private static bool RemovePrefix(ref Task __result, PowerModel? power)
    {
        if (power is not PattyCakePower) return true;

        power.RemoveInternal();
        __result = power.AfterRemoved(power.Owner);

        return false;
    }
}