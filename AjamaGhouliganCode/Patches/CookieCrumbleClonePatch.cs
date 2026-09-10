using AjamaGhouligan.AjamaGhouliganCode.Cards.Rare.Skill;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.CreateClone))]
public static class CookieCrumbleClonePatch
{
    [HarmonyPostfix]
    public static CardModel Postfix(CardModel __result, CardModel __instance)
    {
        if (__instance is not CookieCrumble originalCookie) return __result;
        if (__result is not CookieCrumble newCookie) return __result;

        newCookie.HasBeenPlayed = originalCookie.HasBeenPlayed;

        return newCookie;
    }
}