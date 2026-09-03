using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Extensions;
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
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Common.Attack;

public class Strain() : AjamaGhouliganCard(0,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new OstyDamageVar(6, DamageProps.card),
        new PowerVar<StrainPower>(3),
        new HpLossVar(3)
    ];
    
    public override BundledHoverTipManager MyBundles =>
    [
        BundledHoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (!Osty.CheckMissingWithAnim(Owner))
        {
            await DamageCmd.Attack(DynamicVars.OstyDamage.BaseValue)
                .FromOsty(Owner.Osty!, this, play)
                .Targeting(play.Target!)
                .WithAttackerAnim("attack_poke", 0.3f)
                .WithHitFx(VfxCmd.bluntPath, tmpSfx: TmpSfx.bluntAttack)
                .Execute(choiceContext);
        }

        if (Osty.IsReadyToParty(Owner))
        {
            await CommonActions.Apply<StrainPower>(choiceContext, Owner.Osty!, this);
        }

        await MyActions.OstyLosesHp(choiceContext, this, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<StrainPower>().UpgradeValueBy(1);
    }
}