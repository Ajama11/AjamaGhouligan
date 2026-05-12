using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Screens;

namespace AjamaGhouligan.AjamaGhouliganCode.Patches;

[HarmonyPatch(typeof(NCombatPilesContainer))]
public static class NCombatPilesContainerPatch
{
    [HarmonyPatch("Enable")]
    [HarmonyPostfix]
    public static void EnablePostfix(NCombatPilesContainer __instance)
    {
        __instance.GetNodeOrNull<NBuryPile>("_BuryPile")?.Enable();
    }

    [HarmonyPatch("Disable")]
    [HarmonyPostfix]
    public static void DisablePostfix(NCombatPilesContainer __instance)
    {
        __instance.GetNodeOrNull<NBuryPile>("_BuryPile")?.Disable();
    }
}

[HarmonyPatch(typeof(NCombatUi), "Activate")]
public static class NCombatUiActivatePatch
{
    [HarmonyPostfix]
    public static void ActivatePostfix(NCombatUi __instance, CombatState state)
    {
        var container = __instance.GetNode<NCombatPilesContainer>("%CombatPileContainer");
        var buryPile = container.GetNodeOrNull<NBuryPile>("_BuryPile");
        var player = LocalContext.GetMe(state);
        buryPile?.Initialize(player!);
    }
}

[HarmonyPatch(typeof(NCardPileScreen), "_Ready")]
public static class NCardPileScreenPatch
{
    [HarmonyPostfix]
    public static void ReadyPostfix(NCardPileScreen __instance)
    {
        if (__instance.Pile.Type == SepulchrePile.PileType)
        {
            __instance._bottomLabel.Visible = true;
            __instance._bottomLabel.Text = "[center]" + new LocString("gameplay_ui", "SEPULCHRE_PILE_INFO").GetFormattedText();
        }
    }
}