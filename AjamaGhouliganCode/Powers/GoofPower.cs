using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class GoofPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    private const decimal Threshold = 10;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(3),
        new CardsVar(2)
    ];

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power != this) return;
        if (power.Amount < Threshold) return;
        if (Owner.Player is null) return;

        Flash();

        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner.Player);

        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner.Player);

        await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -Threshold, Owner.Player.Creature, null);
    }
}