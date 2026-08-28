using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
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
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Rare.Attack;

public class MurderOfCrows() : AjamaGhouliganCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9, DamageProps.card),
        new PowerVar<MurderOfCrowsPower>(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        MyEnums.Haunted,
        MyEnums.Grave
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play,
                vfx: VfxCmd.bloodyImpactPath,
                tmpSfx: TmpSfx.daggerThrow)
            .Execute(choiceContext);

        await CommonActions.Apply<MurderOfCrowsPower>(choiceContext,
            CombatState!.HittableEnemies, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<MurderOfCrowsPower>().UpgradeValueBy(1);
    }
}