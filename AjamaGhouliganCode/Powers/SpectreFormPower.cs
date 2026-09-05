using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Cards.Variables;
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

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        int amount = Amount;
        if (cardSource != null) amount++; // To account for the Power card, and also console or cross-mod shenanigans shouldn't have the +1
        
        GetInternalData<Data>().CardsLeft = amount;
        UpdateDisplayAmount();
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.IsAutoPlay) return Task.CompletedTask;
        if (cardPlay.Player.Creature != Owner) return Task.CompletedTask;

        GetInternalData<Data>().CardsLeft--;
        UpdateDisplayAmount();
        
        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.IsAutoPlay) return Task.CompletedTask;
        if (cardPlay.Player.Creature != Owner) return Task.CompletedTask;

        Data data = GetInternalData<Data>();
        if (data.CardsLeft != 1) return Task.CompletedTask;
        
        Flash();
        
        MyActions.GainsHauntedAndEntomb(cardPlay.Card, false);
        
        data.CardsLeft = Amount + 1; // Immediately decremented and invokes display in AfterCardPlayed for the same card play
        
        return Task.CompletedTask;
    }

    public void UpdateDisplayAmount()
    {
        Data data = GetInternalData<Data>();
        
        InvokeDisplayAmountChanged();
        
        ((BoolVar) DynamicVars[NextCard]).BoolVal = data.CardsLeft == 1;
    }

    protected override object InitInternalData() => new Data();
    public class Data
    {
        public int CardsLeft;
    }
}