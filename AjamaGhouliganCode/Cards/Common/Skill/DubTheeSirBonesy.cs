using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Extensions;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Common.Skill;

public class DubTheeSirBonesy() : AjamaGhouliganCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        ..HalfSummon.MakeVars(6, 3),
        new PowerVar<StrengthPower>(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        MyEnums.Haunted
    ];

    public override BundledHoverTipManager MyBundles =>
    [
        BundledHoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        await MyActions.HalfSummon(choiceContext, this);

        if (Osty.IsReadyToParty(Owner))
            await CommonActions.Apply<StrengthPower>(choiceContext, Owner.Osty!, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.HalfSummonFilled.UpgradeValueBy(1);
        DynamicVars.HalfSummonEmpty.UpgradeValueBy(1);
        DynamicVars.Power<StrengthPower>().UpgradeValueBy(1);
    }
}