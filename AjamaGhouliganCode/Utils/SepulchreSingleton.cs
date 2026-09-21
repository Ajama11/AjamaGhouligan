using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
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
        
        foreach (var model in player.Creature.CombatState!.IterateHookListeners())
        {
            if (model is not IAfterSepulchreAutoplayOnTurnStart afterAutoplayModel) continue;
            await afterAutoplayModel.AfterSepulchreAutoplayOnTurnStart(choiceContext, player);
            model.InvokeExecutionFinished();
        }
    }
    
    public override async Task AfterAutoPostPlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        foreach (var model in player.Creature.CombatState!.IterateHookListeners())
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
    
    public static readonly SpireField<CardModel, bool> RemoveFromCurrentAutoplay = new(() => false);

    public static async Task PlayHauntedCardsInSepulchrePile(PlayerChoiceContext choiceContext, Player player)
    {
        List<CardModel> snapshottedSepulchreCards = CardPile.Get(SepulchrePile.PileType, player)!.Cards.ToList();

        foreach (var card in snapshottedSepulchreCards)
        {
            RemoveFromCurrentAutoplay.Set(card, false);
        }
        
        foreach (var card in snapshottedSepulchreCards)
        {
            if (card.Keywords.Contains(MyEnums.Haunted) && !RemoveFromCurrentAutoplay.Get(card))
            {
                await CardCmd.AutoPlay(choiceContext, card, null);
            }
        }
    }
    
    public static async Task PlayAllCardsInSepulchrePile(PlayerChoiceContext choiceContext, Player player)
    {
        foreach (var card in CardPile.Get(SepulchrePile.PileType, player)!.Cards.ToList())
        {
            if (card.Keywords.Contains(CardKeyword.Unplayable)) continue;
            await CardCmd.AutoPlay(choiceContext, card, null);
        }
    }

    public override CardLocation ModifyCardPlayResultLocation(CardModel card, bool isAutoPlay, ResourceInfo resources,
        CardLocation cardLocation)
    {
        if (cardLocation.pileType is PileType.Exhaust or PileType.None) return cardLocation;
        if (!card.Keywords.Contains(MyEnums.Entomb)) return cardLocation;

        cardLocation.pileType = SepulchrePile.PileType;

        return cardLocation;
    }

    public override async Task AfterModifyingCardPlayResultLocation(CardModel card, CardLocation cardLocation)
    {
        foreach (var model in card.CombatState!.IterateHookListeners())
        {
            if (model is not IOnBury onBuryModel) continue;
            await onBuryModel.OnBury(card);
            model.InvokeExecutionFinished();
        }
    }

    public static bool CanGainHaunted(CardModel card)
    {
        return !card.Keywords.Contains(CardKeyword.Unplayable) &&
               !card.Keywords.Contains(MyEnums.Haunted);
    }

    public static bool CanGainEntomb(CardModel card)
    {
        return !card.Keywords.Contains(CardKeyword.Unplayable) &&
               !card.Keywords.Contains(MyEnums.Entomb) &&
               !card.Keywords.Contains(CardKeyword.Exhaust) &&
               card.Type != CardType.Power &&
               !card.IsDupe;
    }
}