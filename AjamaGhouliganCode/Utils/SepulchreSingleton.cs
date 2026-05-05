using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public class SepulchreSingleton() : CustomSingletonModel(true, false)
{
    public override async Task AfterAutoPrePlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        foreach (var card in CardPile.Get(SepulchrePile.PileType, player)!.Cards.ToList())
        {
            if (card.Keywords.Contains(GhouliganEnums.Haunted))
            {
                await CardCmd.AutoPlay(choiceContext, card, null);
            }
        }
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (card.HasBeenRemovedFromState || card.Pile == null) return Task.CompletedTask;

        if (card.Pile!.Type != SepulchrePile.PileType && 
            card.Pile!.Type != PileType.Play && 
            card.Pile!.Type != PileType.None &&
            card.Keywords.Contains(GhouliganEnums.Buried))
        {
            CardCmd.RemoveKeyword(card, GhouliganEnums.Buried);
        }

        if (card.Pile!.Type == SepulchrePile.PileType)
        {
            CardCmd.ApplyKeyword(card, GhouliganEnums.Buried);
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card is not AjamaGhouliganCard && 
            cardPlay.Card.Type != CardType.Power &&
            cardPlay.Card.Keywords.Contains(GhouliganEnums.Bury) &&
            !cardPlay.Card.Keywords.Contains(CardKeyword.Exhaust) &&
            cardPlay.IsLastInSeries)
        {
            await CardPileCmd.Add(cardPlay.Card, SepulchrePile.PileType);
        }
    }
}