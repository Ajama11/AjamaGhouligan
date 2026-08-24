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

public class PartnersInCrime() : AjamaGhouliganCard(1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    private const string Threshold = "Threshold";
    private const string HitCount = "HitCount";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4, DamageProps.card),
        new IntVar(Threshold, 10),
        ..MakeCalculatedVar(HitCount, 2, (card, _) =>
            Math.Floor(card.Owner.Osty?.MaxHp / card.DynamicVars[Threshold].BaseValue ?? 0))
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play,
                (int) ((CalculatedVar) DynamicVars[HitCount]).Calculate(null),
                VfxCmd.rockShatterPath,
                tmpSfx: TmpSfx.bluntAttack)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}