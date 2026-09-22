using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class SpectreFormPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override int DisplayAmount => GetInternalData<Data>().CardsLeft;

    private const string NextCard = "NextCard";
    private const string Display = "Display";
    
    private CardModel? CardSource { get; set; }
    private CardModel? CardToEntombImmediately { get; set; }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BoolVar(NextCard, false),
        new DisplayVar<SpectreFormPower>(Display, p => p.DisplayAmount.ToString())
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(MyEnums.Haunted),
        HoverTipFactory.FromKeyword(MyEnums.Entomb)
    ];
    
    public void InitializeCardsLeft(int amount)
    {
        GetInternalData<Data>().CardsLeft = amount;
        UpdateDisplayAmount();
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        // In case values should change from Hooks, or the power was applied through other means
        InitializeCardsLeft(Amount);
        
        if (cardSource != null) CardSource = cardSource;
        
        return Task.CompletedTask;
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.IsAutoPlay) return;
        if (cardPlay.Player.Creature != Owner) return;
        
        if (CardSource != null && cardPlay.Card == CardSource)
        {
            // If another mod lets a Power card escape being removed from combat, this should only early return on the card's 1st play.
            // I know Pengo's Tarot mod can, or used to, let any card return to the Hand once, and it works/worked on Powers.
            CardSource = null;
            return;
        }

        Data data = GetInternalData<Data>();
        
        data.CardsLeft--;
        UpdateDisplayAmount();

        if (CardToEntombImmediately == cardPlay.Card)
        {
            await CardPileCmd.Add(cardPlay.Card, SepulchrePile.PileType);
            CardToEntombImmediately = null;
        }
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.IsAutoPlay) return Task.CompletedTask;
        if (cardPlay.Player.Creature != Owner) return Task.CompletedTask;

        Data data = GetInternalData<Data>();
        if (data.CardsLeft != 1) return Task.CompletedTask;
        
        Flash();
        
        MyActions.GainsHauntedAndEntomb(cardPlay.Card, false);
        CardToEntombImmediately = cardPlay.Card;
        
        data.CardsLeft = Amount + 1; // Immediately decremented and invokes display in AfterCardPlayed for the same card play
        
        return Task.CompletedTask;
    }

    public void UpdateDisplayAmount()
    {
        Data data = GetInternalData<Data>();
        
        InvokeDisplayAmountChanged();
        
        ((BoolVar) DynamicVars[NextCard]).BoolVal = data.CardsLeft == 1;

        if (data.CardsLeft == 1)
            StartPulsing();
        else
            StopPulsing();
    }

    protected override object InitInternalData() => new Data();
    public class Data
    {
        public int CardsLeft;
    }
}