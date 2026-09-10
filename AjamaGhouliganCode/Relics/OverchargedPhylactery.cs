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
        ..HalfSummon.MakeVars(14, 7)
    ];

    public override bool SpawnsPets => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HalfSummon.DynamicTip(DynamicVars)
    ];

    public override async Task BeforeCombatStart()
    {
        await MyActions.HalfSummon(this, Owner,
            DynamicVars.HalfSummonFilled.IntValue,
            DynamicVars.HalfSummonEmpty.IntValue,
            new ThrowingPlayerChoiceContext());
    }
}