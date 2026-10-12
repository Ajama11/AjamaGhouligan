using System.Reflection.Emit;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;

namespace AjamaGhouligan.AjamaGhouliganCode.Patches;

// Adapted from Blizzare's The Electron

[HarmonyPatch()]
public static class SpectreFormGlowPatch
{
    private static readonly Color LightGold = new ("ffeca6fa");
    private static readonly Color Pink = new ("fc77edfa");
    
    [HarmonyPatch(typeof(NHandCardHolder), nameof(NHandCardHolder.UpdateCard))]
    public static class NHandCardHolderUpdateCardPatch
    {
        [HarmonyPostfix]
        private static void Postfix(NHandCardHolder __instance)
        {
            var card = __instance.CardNode?.Model;
            if (card == null || !card.CanPlay() || card.ShouldGlowRed)
                return;

            if (ShouldGlowPink(card))
            {
                __instance.CardNode?.CardHighlight.Modulate = card.ShouldGlowGold ?
                    LightGold :
                    Pink;
            }
        }
    }

    [HarmonyPatch(typeof(NHandCardHolder), nameof(NHandCardHolder.Flash))]
    public static class NHandCardHolderFlashPatch
    {
        [HarmonyPostfix]
        private static void Postfix(NHandCardHolder __instance)
        {
            var card = __instance.CardNode?.Model;
            if (card == null || !card.CanPlay() || card.ShouldGlowRed)
                return;

            if (ShouldGlowPink(card))
            {
                __instance._flash.Modulate = card.ShouldGlowGold ?
                    LightGold :
                    Pink;
            }
        }
    }
    
    static bool ShouldGlowPink(CardModel cardModel)
    {
        var spectreFormPowers = cardModel.Owner.Creature.Powers
            .Where(p => p is SpectreFormPower);
        
        return spectreFormPowers.Any(p =>
            p.GetInternalData<SpectreFormPower.Data>().CardsLeft == 1);
    }
}