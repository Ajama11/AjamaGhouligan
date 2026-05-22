using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class MisfortunePower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override Color AmountLabelColor => _normalAmountLabelColor;
    
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (CombatManager.Instance.IsOverOrEnding || side != Owner.Side || Owner.IsDead) return;

        await PowerCmd.ModifyAmount(choiceContext, this, -1 * (decimal) Math.Ceiling(Amount * 0.5), Applier, null);
    }
}