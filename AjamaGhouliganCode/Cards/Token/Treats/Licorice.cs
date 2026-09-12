using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Token.Treats;

public class Licorice() : BaseTreat()
{
    protected override IEnumerable<DynamicVar> TreatCanonicalVars =>
    [
        new PowerVar<MisfortunePower>(2)
    ];

    protected override async Task TreatOnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await MyActions.Misfortune(choiceContext, CombatState!.HittableEnemies, this);
    }

    protected override void TreatOnUpgrade()
    {
        AddKeyword(MyEnums.Unfortunate);
    }
}