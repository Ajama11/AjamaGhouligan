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

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner) return Task.CompletedTask;
        
        GetInternalData<Data>().AmountsForPlayedCards.Add(cardPlay.Card, Amount);
        
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.IsAutoPlay) return;
        if (cardPlay.Player.Creature != Owner) return;
        
        Data data = GetInternalData<Data>();
        
        if (!data.AmountsForPlayedCards.Remove(cardPlay.Card, out var storedAmount) || storedAmount <= 0)
            return;
        
        if (CombatManager.Instance.History
                .CardPlaysStarted
                .Count(e => 
                    e.Actor == Owner && 
                    e.CardPlay is { IsFirstInSeries: true, IsAutoPlay: false } &&
                    e.HappenedThisTurn(CombatState)
                ) > storedAmount)
            return;

        Flash();

        UpdateSecondAmount();

        CardModel clone = cardPlay.Card.CreateClone();
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(clone, SepulchrePile.PileType, Owner.Player), 0.6F);
    }
    
    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
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
        public readonly Dictionary<CardModel, int> AmountsForPlayedCards = [];
    }
}