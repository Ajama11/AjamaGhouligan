using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class TastySoulPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    private const string ApplierName = "ApplierName";
    private const string NumberOfFeasts = "NumberOfFeasts";

    private IEnumerable<CardModel> _cardsToExhaust = [];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar(ApplierName),
        new IntVar(NumberOfFeasts, 0)
    ];

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (cardSource != null)
        {
            _cardsToExhaust = [cardSource, .._cardsToExhaust];
            ((IntVar) DynamicVars[NumberOfFeasts]).BaseValue++;
        }
        
        if (applier is { IsMonster: true })
            ((StringVar) DynamicVars[ApplierName]).StringValue = applier.Monster!.Title.GetFormattedText();
        
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power != this) return Task.CompletedTask;
        
        if (cardSource != null)
        {
            _cardsToExhaust = [cardSource, .._cardsToExhaust];
            ((IntVar) DynamicVars[NumberOfFeasts]).BaseValue++;
        }
        
        return Task.CompletedTask;
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented) return;
        if (Applier == null) return;
        if (creature != Owner) return;
        if (!creature.Powers.All(p => p.ShouldOwnerDeathTriggerFatal())) return;

        await CreatureCmd.GainMaxHp(Applier, Amount);

        foreach (CardModel card in _cardsToExhaust)
        {
            if (card.Pile != null && card.Pile.Type != PileType.Exhaust)
                await CardCmd.Exhaust(choiceContext, card);
        }

        await PowerCmd.Remove(this);
    }

    public override async Task AfterSideTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        
        await PowerCmd.Remove(this);
    }
}