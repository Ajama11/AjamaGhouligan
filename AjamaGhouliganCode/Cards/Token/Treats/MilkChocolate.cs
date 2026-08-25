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
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Token.Treats;

public class MilkChocolate() : BaseTreat()
{
    protected override IEnumerable<DynamicVar> TreatCanonicalVars =>
    [
        new SummonVar(6)
    ];

    protected override async Task TreatOnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await MyActions.Summon(choiceContext, this);
    }

    protected override void TreatOnUpgrade()
    {
        DynamicVars.Summon.UpgradeValueBy(3);
    }
}