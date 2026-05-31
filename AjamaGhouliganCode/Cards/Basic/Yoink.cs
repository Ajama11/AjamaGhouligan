using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Ancient;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Basic;

public class Yoink() : AjamaGhouliganCard(1,
    CardType.Skill, CardRarity.Basic,
    TargetType.Self),
    ITranscendenceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SummonVar(5)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        await MyActions.Summon(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Summon.UpgradeValueBy(2);
    }
    
    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<Yoinkadoo>();
    }
}