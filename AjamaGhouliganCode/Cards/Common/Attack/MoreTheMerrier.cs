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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Common.Attack;

public class MoreTheMerrier() : AjamaGhouliganCard(0,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy),
    IOnDisinter
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5, DamageProps.card),
        new DisinterVar(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        MyEnums.Grave,
        MyEnums.Bury
    ];

    public override IEnumerable<IHoverTip> MyHoverTips =>
    [
        HoverTipFactory.Static(MyEnums.Disinter),
        HoverTipFactory.FromKeyword(MyEnums.Haunted)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play,
                1,
                VfxCmd.gazePath)
            .Execute(choiceContext);
    }
    
    public async Task OnDisinter(CardModel card)
    {
        if (card != this) return;

        await MyActions.DisinterRandomNonHaunted(this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars.Disinter.UpgradeValueBy(1);
    }
}