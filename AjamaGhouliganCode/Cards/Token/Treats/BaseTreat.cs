using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Token.Treats;

[Pool(typeof(TokenCardPool))]
public abstract class BaseTreat() : AjamaGhouliganCard(0,
    CardType.Skill, CardRarity.Token,
    TargetType.Self)
{
    protected virtual IEnumerable<DynamicVar> TreatCanonicalVars => [];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new LoseDoomVar(2),
        new HealVar(2),
        ..TreatCanonicalVars
    ];
    
    public virtual IEnumerable<CardKeyword> TreatCanonicalKeywords => [];
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust,
        ..TreatCanonicalKeywords
    ];
    
    public override HashSet<CardTag> MyCanonicalTags =>
    [
        MyEnums.Treat
    ];
    
    public virtual BundledHoverTipManager TreatMyBundles => [];
    public override BundledHoverTipManager MyBundles =>
    [
        ..TreatMyBundles
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        await MyActions.LoseDoom(choiceContext, this);
        await MyActions.OstyHeal(this);

        await TreatOnPlay(choiceContext, play);
    }

    protected virtual Task TreatOnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        return Task.CompletedTask;
    }

    protected virtual void TreatOnUpgrade() { }
    protected override void OnUpgrade()
    {
        DynamicVars.LoseDoom.UpgradeValueBy(1);
        DynamicVars.Heal.UpgradeValueBy(1);
        TreatOnUpgrade();
    }
}