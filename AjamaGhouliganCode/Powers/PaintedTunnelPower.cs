using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class PaintedTunnelPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<MisfortunePower>()
    ];

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier,
        out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (canonicalPower is not MisfortunePower) return false;
        if (target != Owner) return false;
        if (amount >= 0) return false;
        
        modifiedAmount = 0;
        
        return true;
    }

    public override async Task AfterSideTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;

        await PowerCmd.Decrement(this);
    }
}