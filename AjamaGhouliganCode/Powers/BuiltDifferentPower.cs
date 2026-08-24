using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class BuiltDifferentPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VigorPower>()
    ];

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature.Monster is not Osty osty) return;
        if (osty.Creature.PetOwner?.Creature != Owner) return;
        if (delta <= 0) return;

        await PowerCmd.Apply<VigorPower>(new ThrowingPlayerChoiceContext(),
            osty.Creature, Amount * delta,
            Owner, null);
    }
}