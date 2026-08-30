using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class FrightPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card.Owner.Creature != Owner) return;
        if (card.Type != CardType.Status) return;
        if (card.Pile?.Type != PileType.Hand) return;

        Creature? target = Owner.Player!.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        
        if (target == null) return;
        
        Flash();

        var vfx = NSweepingBeamImpactVfx.Create(target);
        vfx?.SetModulate(new Color("1f0026"));
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
        
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(),
            target, Amount, DamageProps.nonCardHpLoss, 
            null, null);
    }
}