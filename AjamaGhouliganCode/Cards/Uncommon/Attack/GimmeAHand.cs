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
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Uncommon.Attack;

public class GimmeAHand() : AjamaGhouliganCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SummonVar(5),
        new OstyDamageVar(9, ValueProp.Move),
        new LoseDoomVar(3, true)
    ];
    
    public override IEnumerable<IHoverTip> MyHoverTips =>
        IsUpgraded ? [HoverTipFactory.FromPower<DoomPower>()] : [];

    protected override bool ShouldGlowRedInternal => false;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await MyActions.Summon(choiceContext, this);

        if (!Osty.CheckMissingWithAnim(Owner) && play.Target != null)
        {
            await DamageCmd.Attack(DynamicVars.OstyDamage.BaseValue)
                .FromOsty(Owner.Osty!, this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
                .Execute(choiceContext);
        }

        if (IsUpgraded) await MyActions.LoseDoom(choiceContext, this);
    }
}