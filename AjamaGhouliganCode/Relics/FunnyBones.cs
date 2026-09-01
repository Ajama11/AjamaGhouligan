using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Relics;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Relics;

public class FunnyBones() : AjamaGhouliganRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public bool StrengthApplied
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        StrengthApplied = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        if (creature != Owner.Creature) return;
        await ModifyStrengthIfNecessary();
    }

    public override async Task AfterBlockCleared(Creature creature)
    {
        if (creature != Owner.Creature) return;
        await ModifyStrengthIfNecessary();
    }

    public override async Task AfterBlockBroken(PlayerChoiceContext choiceContext, Creature target, Creature? breaker)
    {
        if (target != Owner.Creature) return;
        await ModifyStrengthIfNecessary();
    }

    public override async Task AfterOstyRevived(Creature osty)
    {
        if (osty != Owner.Osty) return;
        await ModifyStrengthIfNecessary();
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature != Owner.Osty) return;
        await ModifyStrengthIfNecessary();
    }

    public async Task ModifyStrengthIfNecessary()
    {
        bool hasBlock = Owner.Creature.Block > 0;
        bool isOstyAlive = Owner.IsOstyAlive;
        decimal strengthAmount = DynamicVars.Strength.BaseValue;

        bool shouldDisable = !hasBlock || !isOstyAlive;

        Status = shouldDisable ? RelicStatus.Normal : RelicStatus.Active;

        if (shouldDisable && StrengthApplied)
        {
            Flash();
            
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(),
                Owner.Creature, -strengthAmount,
                Owner.Creature, null);

            if (isOstyAlive)
            {
                await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(),
                    Owner.Osty!, -strengthAmount,
                    Owner.Osty, null);
            }

            StrengthApplied = false;
        }
        else if (!shouldDisable && !StrengthApplied)
        {
            Flash();
            
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(),
                Owner.Creature, strengthAmount,
                Owner.Creature, null);
            
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(),
                Owner.Osty!, strengthAmount,
                Owner.Osty, null);

            StrengthApplied = true;
        }
    }
}