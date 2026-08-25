using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaGhouligan.AjamaGhouliganCode.Relics;

public class OverchargedPhylactery : AjamaGhouliganRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        ..HalfSummon.MakeVars(14, 10),
        new LoseDoomVar(3)
    ];

    public override bool SpawnsPets => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HalfSummon.DynamicTip(DynamicVars),
        HoverTipFactory.FromPower<DoomPower>()
    ];

    public override async Task BeforeCombatStart()
    {
        await MyActions.HalfSummon(this, Owner,
            DynamicVars.HalfSummonFilled.IntValue,
            DynamicVars.HalfSummonEmpty.IntValue,
            new ThrowingPlayerChoiceContext());
    }

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        if (!player.Creature.HasPower<DoomPower>()) return;
        
        Flash();

        await MyActions.LoseDoom(Owner.Creature, DynamicVars.LoseDoom.IntValue, choiceContext);
    }
}