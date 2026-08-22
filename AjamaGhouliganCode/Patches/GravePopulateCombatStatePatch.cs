using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Patches.Content;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Patches;

[HarmonyPatch(typeof(Player))]
public static class GravePopulateCombatStatePatch
{
    [HarmonyPatch(nameof(Player.PopulateCombatState))]
    [HarmonyPostfix]
    public static void Postfix(Player __instance)
    {
        List<CardModel> graveCards = [];
        
        foreach (var card in __instance.PlayerCombatState!.DrawPile.Cards.ToList())
        {
            if (card.Keywords.Contains(MyEnums.Grave))
            {
                graveCards.Add(card);
            }
        }

        graveCards = graveCards
            .OrderBy(c => c.Owner.Deck.Cards.IndexOf(c.DeckVersion))
            .ToList();

        foreach (var card in graveCards)
        {
            card.RemoveFromCurrentPile();
            CustomPiles.GetCustomPile(__instance.PlayerCombatState, SepulchrePile.PileType)!.AddInternal(card);
        }
    }
}