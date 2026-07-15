using AjamaGhouligan.AjamaGhouliganCode.Character;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Patches;

[HarmonyPatch(typeof(CharacterModel), nameof(CharacterModel.PowerUpSfx), MethodType.Getter)]
public static class CustomPowerUpSfxPatch
{
    [HarmonyPrefix]
    static bool Custom(CharacterModel __instance, ref string? __result)
    {
        if (__instance is not Ghouligan ghouligan)
            return true;

        __result = ghouligan.CustomPowerUpSfx;
        return false;
    }
}