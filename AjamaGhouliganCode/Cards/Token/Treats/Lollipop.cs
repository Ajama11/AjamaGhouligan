using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Token.Treats;

public class Lollipop() : BaseTreat()
{
    public override TargetType TargetType => TargetType.AnyEnemy;

    protected override IEnumerable<DynamicVar> TreatCanonicalVars =>
    [
        new PowerVar<StrengthPower>(3)
    ];

    public override BundledHoverTipManager TreatMyBundles =>
    [
        BundledHoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override async Task TreatOnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<LollipopPower>(choiceContext, play.Target!,
            DynamicVars.Power<StrengthPower>().BaseValue,
            Owner.Creature, this);
    }

    protected override void TreatOnUpgrade()
    {
        DynamicVars.Power<StrengthPower>().UpgradeValueBy(2);
    }
}