using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Basic;

public class Boop() : AjamaGhouliganCard(0,
    CardType.Attack, CardRarity.Basic,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new OstyDamageVar(4, DamageProps.card)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        MyEnums.Haunted,
        MyEnums.Unfortunate
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (Osty.CheckMissingWithAnim(Owner)) return;
        
        await DamageCmd.Attack(DynamicVars.OstyDamage.BaseValue)
            .FromOsty(Owner.Osty!, this, play)
            .Targeting(play.Target!)
            .WithAttackerAnim(Osty.pokeAnim, 0.3f)
            .WithHitFx(VfxCmd.bluntPath, tmpSfx: TmpSfx.bluntAttack)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(MyEnums.Entomb);
    }
}