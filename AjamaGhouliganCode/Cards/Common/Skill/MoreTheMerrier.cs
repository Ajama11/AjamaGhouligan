using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Common.Skill;

public class MoreTheMerrier() : AjamaGhouliganCard(0,
    CardType.Skill, CardRarity.Common,
    TargetType.Self),
    IOnDisinter
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SummonVar(4),
        new DisinterVar(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        MyEnums.Grave,
        MyEnums.Bury
    ];

    public override BundledHoverTipManager MyBundles =>
    [
        BundledHoverTipFactory.FromKeyword(MyEnums.Haunted)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await MyActions.Summon(choiceContext, this);
    }
    
    public async Task OnDisinter(CardModel card)
    {
        if (card != this) return;

        await MyActions.DisinterRandomNonHaunted(this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Summon.UpgradeValueBy(2);
        DynamicVars.Disinter.UpgradeValueBy(1);
    }
}