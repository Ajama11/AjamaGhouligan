using BaseLib.Cards.Variables;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class MisfortunePower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public const string WhenTriggered = "WhenTriggered";
    public const string Display = "Display";

    public const decimal DefaultValue = -2;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new (WhenTriggered, DefaultValue),
        new DisplayVar<MisfortunePower>(Display, p => Math.Abs(p.DynamicVars[WhenTriggered].IntValue).ToString())
    ];

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (CombatState.PlayerCreatures.Any(player => player.HasPower<WildRidePower>()))
        {
            DynamicVars[WhenTriggered].BaseValue = GetTotalWildRide();
        }

        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power is not WildRidePower) return Task.CompletedTask;

        DynamicVars[WhenTriggered].BaseValue = GetTotalWildRide();
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets the total amount of Wild Ride across all players.
    /// If it's somehow 0 when this is called, from the power being removed by another mod, then returns the default of -2
    /// </summary>
    public decimal GetTotalWildRide()
    {
        decimal amount = CombatState.PlayerCreatures.Sum(p => p.GetPowerAmount<WildRidePower>());
        
        return amount > 0 ? amount : DefaultValue;
    }
}