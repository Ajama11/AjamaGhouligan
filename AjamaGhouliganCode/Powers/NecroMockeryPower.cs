using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class NecroMockeryPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(MyEnums.Unfortunate),
        HoverTipFactory.FromPower<MisfortunePower>()
    ];

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
       if (delta >= 0 || creature.Monster is not Osty || creature.PetOwner != Owner.Player || Owner.Player == null) return;
       
       Creature? target = Owner.Player.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
       
       if (target == null) return;
       
       Flash();

       await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target,
           -delta * Amount + target.GetPowerAmount<MisfortunePower>(), 
           ValueProp.Unpowered, Owner, null, null);
    }
}