using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Uncommon.Attack;

public class GimmeAHand() : AjamaGhouliganCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy),
    IOnDisinter
{
    private const string Increase = "Increase";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        ..HalfSummon.MakeVars(4, 5),
        new OstyDamageVar(4, ValueProp.Move),
        new (Increase, 5)
    ];

    public decimal ExtraDamageFromDisinter
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        MyEnums.Grave
    ];

    public override IEnumerable<IHoverTip> MyHoverTips =>
    [
        HalfSummon.DynamicTip(DynamicVars),
        HoverTipFactory.Static(MyEnums.Disinter)
    ];

    protected override bool ShouldGlowRedInternal => false;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await MyActions.HalfSummon(choiceContext, this);

        if (!Osty.CheckMissingWithAnim(Owner) && play.Target != null)
        {
            await DamageCmd.Attack(DynamicVars.OstyDamage.BaseValue)
                .FromOsty(Owner.Osty!, this, play)
                .Targeting(play.Target)
                .WithHitFx(VfxCmd.bluntPath, tmpSfx: TmpSfx.bluntAttack)
                .Execute(choiceContext);
        }
    }
    
    public async Task OnDisinter(CardModel card)
    {
        if (card != this) return;

        DynamicVars.OstyDamage.BaseValue += DynamicVars[Increase].BaseValue;
        ExtraDamageFromDisinter += DynamicVars[Increase].BaseValue;
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.OstyDamage.BaseValue += ExtraDamageFromDisinter;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.HalfSummonFilled.UpgradeValueBy(2);
        DynamicVars.HalfSummonTotal.UpgradeValueBy(3);
        DynamicVars[Increase].UpgradeValueBy(5);
    }
}