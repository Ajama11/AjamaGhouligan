using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class BakersHandPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..new PattyCakeBundle(2).HoverTips,
        HoverTipFactory.FromPower<VigorPower>()
    ];

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.Owner != Owner) return;
        if (amount <= 0) return;
        if (power is not PattyCakePower) return;
        
        Flash();

        foreach (Creature otherPlayer in CombatState!
                     .GetTeammatesOf(Owner)
                     .Where(c =>
                         c is { IsAlive: true, IsPlayer: true } &&
                         c != Owner))
        {
            await PowerCmd.Apply<VigorPower>(choiceContext,
                otherPlayer, Amount * amount,
                Owner, null);
        }
    }
}