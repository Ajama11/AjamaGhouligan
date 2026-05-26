using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public class SepulchreSingleton() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterAutoPrePlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        await PlayHauntedCardsInSepulchrePile(choiceContext, player);

        CardModel? someCard = player.PlayerCombatState!.AllCards.FirstOrDefault();
        if (someCard == null) return;
        
        foreach (var model in someCard.CombatState!.IterateHookListeners())
        {
            if (model is not IAfterSepulchreAutoplayOnTurnStart afterAutoplayModel) continue;
            await afterAutoplayModel.AfterSepulchreAutoplayOnTurnStart(choiceContext, player);
            model.InvokeExecutionFinished();
        }
    }
    
    public override async Task AfterAutoPostPlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        CardModel? someCard = player.PlayerCombatState!.AllCards.FirstOrDefault();
        if (someCard == null) return;
        
        foreach (var model in someCard.CombatState!.IterateHookListeners())
        {
            if (model is not IBeforeSepulchreAutoplayOnTurnEnd beforeAutoplayModel) continue;
            await beforeAutoplayModel.BeforeSepulchreAutoplayOnTurnEnd(choiceContext, player);
            model.InvokeExecutionFinished();
        }
        
        if (player.Creature.HasPower<EclipsePower>())
        {
            await PlayHauntedCardsInSepulchrePile(choiceContext, player);
        }
    }

    public static async Task PlayHauntedCardsInSepulchrePile(PlayerChoiceContext choiceContext, Player player)
    {
        foreach (var card in CardPile.Get(SepulchrePile.PileType, player)!.Cards.ToList())
        {
            if (card.Keywords.Contains(MyEnums.Haunted))
            {
                await CardCmd.AutoPlay(choiceContext, card, null);
            }
        }
    }
    
    public static async Task PlayAllCardsInSepulchrePile(PlayerChoiceContext choiceContext, Player player)
    {
        foreach (var card in CardPile.Get(SepulchrePile.PileType, player)!.Cards.ToList())
        {
            await CardCmd.AutoPlay(choiceContext, card, null);
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Power &&
            cardPlay.Card.Keywords.Contains(MyEnums.Bury) &&
            !cardPlay.Card.Keywords.Contains(CardKeyword.Exhaust) &&
            cardPlay.IsLastInSeries)
        {
            await CardPileCmd.Add(cardPlay.Card, SepulchrePile.PileType);
            
            foreach (var model in cardPlay.Card.CombatState!.IterateHookListeners())
            {
                if (model is not IOnBury onBuryModel) continue;
                await onBuryModel.OnBury(cardPlay.Card);
                model.InvokeExecutionFinished();
            }
        }
    }
}