using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class CookieCrumblePower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    public override int DisplayAmount => GetInternalData<Data>().CardsLeft;
    
    private const string Display = "Display";
    
    private CardModel? CardSource { get; set; }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new MaxHpVar(3),
        new DisplayVar<CookieCrumblePower>(Display, p => p.DisplayAmount.ToString())
    ];
    
    public void InitializeValues(int amount, decimal maxHp)
    {
        GetInternalData<Data>().CardsLeft = amount;
        DynamicVars.MaxHp.BaseValue = maxHp;
        UpdateDisplayAmount();
    }
    
    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        // In case values should change from Hooks, or the power was applied through other means
        InitializeValues(Amount, DynamicVars.MaxHp.BaseValue);
        
        if (cardSource != null) CardSource = cardSource;
        
        return Task.CompletedTask;
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
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

        if (data.CardsLeft == 0)
        {
            Flash();
            
            if (LocalContext.IsMe(Owner))
                VfxCmd.PlayFullScreenInCombat(VfxCmd.bitePath, Owner);
            
            await CreatureCmd.GainMaxHp(cardPlay.Player.Creature, DynamicVars.MaxHp.BaseValue);
            await PowerCmd.Remove(this);
        }
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return Task.CompletedTask;
        
        GetInternalData<Data>().CardsLeft = Amount;
        UpdateDisplayAmount();
        
        return Task.CompletedTask;
    }

    public void UpdateDisplayAmount()
    {
        InvokeDisplayAmountChanged();

        if (GetInternalData<Data>().CardsLeft <= 1)
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