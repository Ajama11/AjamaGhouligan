using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Abstracts;
using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class TwinsiesPower : AjamaGhouliganPower, IHasSecondAmount
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.IsAutoPlay) return;
        if (cardPlay.Card.Owner.Creature != Owner) return;
        
        Data data = GetInternalData<Data>();
        
        if (!data.ShouldCopyTwinsies && data.CardSources.Contains(cardPlay.Card)) return;
        
        if (CombatManager.Instance.History
                .CardPlaysStarted
                .Count(e => 
                    e.Actor == Owner && 
                    e.CardPlay is { IsFirstInSeries: true, IsAutoPlay: false } &&
                    e.HappenedThisTurn(CombatState)
                ) > Amount)
            return;

        Flash();

        UpdateSecondAmount();

        CardModel clone = cardPlay.Card.CreateClone();
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(clone, SepulchrePile.PileType, Owner.Player), 0.6F);
    }
    
    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        Data data = GetInternalData<Data>();
        
        if (cardSource != null) data.CardSources.Add(cardSource);
        data.ShouldCopyTwinsies = false;
        
        UpdateSecondAmount();
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return Task.CompletedTask;
        
        UpdateSecondAmount();
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power != this) return Task.CompletedTask;
        
        Data data = GetInternalData<Data>();
        if (cardSource != null) data.CardSources.Add(cardSource);
        data.ShouldCopyTwinsies = data.CardsLeft > 0;
        
        UpdateSecondAmount();
        return Task.CompletedTask;
    }

    public void UpdateSecondAmount()
    {
        Data data = GetInternalData<Data>();
        
        data.CardsLeft = Amount - CombatManager.Instance.History
            .CardPlaysStarted
            .Count(e => 
                e.Actor == Owner && 
                e.CardPlay is { IsFirstInSeries: true, IsAutoPlay: false } &&
                e.HappenedThisTurn(CombatState)
            );
        this.InvokeSecondAmountChanged();
    }

    public string GetSecondAmount()
    {
        Data data = GetInternalData<Data>();
        return Math.Max(0, data.CardsLeft).ToString();
    }
    
    protected override object InitInternalData() => new Data();

    public class Data
    {
        public int CardsLeft;
        public bool ShouldCopyTwinsies;
        public List<CardModel> CardSources = [];
    }
}