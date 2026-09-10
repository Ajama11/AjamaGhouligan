using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaGhouligan.AjamaGhouliganCode.Relics;

public class StolenPhylactery : AjamaGhouliganRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    public override RelicModel GetUpgradeReplacement() => ModelDb.Relic<OverchargedPhylactery>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        ..HalfSummon.MakeVars(6, 4)
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