using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.Relics;

public class OverchargedPhylactery : AjamaGhouliganRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SummonVar(14),
        new LoseDoomVar(3)
    ];

    public override bool SpawnsPets => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.SummonDynamic, DynamicVars.Summon)
    ];

    public override async Task BeforeCombatStart()
    {
        await OstyCmd.Summon(new ThrowingPlayerChoiceContext(), Owner, DynamicVars.Summon.BaseValue, this);
    }

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        
        Flash();

        await MyActions.LoseDoom(Owner.Creature, DynamicVars.LoseDoom.IntValue, choiceContext);
    }
}