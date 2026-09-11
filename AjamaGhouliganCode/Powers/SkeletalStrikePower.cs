using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class SkeletalStrikePower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SummonVar(2)
    ];

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (delta >= 0 || creature.Monster is not Osty || creature.PetOwner != Owner.Player || Owner.Player == null) return;
        if (creature.IsDead) return;

        float waitTime = creature.CombatState!.CurrentSide == CombatSide.Player ?
            0.2f : 0.1f;

        await Cmd.Wait(waitTime);

        await DoTheThing();
    }
    
    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature.Monster is not Osty || creature.PetOwner != Owner.Player || Owner.Player == null) return;
        
        float waitTime = creature.CombatState!.CurrentSide == CombatSide.Player ?
            deathAnimLength * 0.5f : 0.1f;

        await Cmd.Wait(waitTime);
        
        await DoTheThing();
    }

    private async Task DoTheThing()
    {
        Flash();

        await MyActions.Summon(this, Owner.Player!, DynamicVars.Summon.IntValue);
    }
    
    public override async Task AfterSideTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Enemy) return;

        await PowerCmd.Decrement(this);
    }
}