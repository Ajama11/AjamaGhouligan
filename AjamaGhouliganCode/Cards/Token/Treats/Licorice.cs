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
    public override IEnumerable<CardKeyword> TreatCanonicalKeywords =>
    [
        MyEnums.Unfortunate
    ];

    protected override async Task TreatOnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (IsUpgraded)
            await UnfortunateSingleton.Trigger(CombatState!, 1, choiceContext, play);
    }
}