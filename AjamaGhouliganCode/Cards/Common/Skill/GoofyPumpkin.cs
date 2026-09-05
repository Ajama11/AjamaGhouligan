using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Common.Skill;

public class GoofyPumpkin() : AjamaGhouliganCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<GoofPower>(4),
        new CardsVar(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    public override BundledHoverTipManager MyBundles =>
    [
        BundledHoverTipFactory.FromKeyword(MyEnums.Haunted),
        BundledHoverTipFactory.FromKeyword(MyEnums.Entomb)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await MyActions.Goof(choiceContext, this);
        
        MyActions.GainsHauntedAndEntomb((await CommonActions.SelectCards(this,
                MySelectionPrompts.AddHauntedAndEntomb, choiceContext,
                PileType.Hand,
                c =>
                    !(c.Keywords.Contains(MyEnums.Haunted) &&
                      c.Keywords.Contains(MyEnums.Entomb)),
                DynamicVars.Cards.IntValue))
            .ToList());
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<GoofPower>().UpgradeValueBy(2);
    }
}