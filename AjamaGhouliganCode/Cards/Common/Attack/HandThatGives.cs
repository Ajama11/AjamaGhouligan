using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Audio;
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

public class HandThatGives() : AjamaGhouliganCard(0,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new OstyDamageVar(3, DamageProps.card),
        new EnergyVar(1),
        new SurpriseVar(1)
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
                .WithHitFx(VfxCmd.bluntPath)
                .Execute(choiceContext);
        }

        await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, 
            Owner.Creature, DynamicVars.Energy.BaseValue, 
            Owner.Creature, this);

        await MyActions.CreateSurprises(this, PileType.Discard, CardPilePosition.Bottom);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Surprise.UpgradeValueBy(1);
    }
}