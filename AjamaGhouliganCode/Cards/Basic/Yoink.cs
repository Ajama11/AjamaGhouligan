using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Ancient;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
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
        ..HalfSummon.MakeVars(5, 2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        await MyActions.HalfSummon(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.HalfSummonFilled.UpgradeValueBy(2);
        DynamicVars.HalfSummonEmpty.UpgradeValueBy(1);
    }
    
    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<Yoinkadoo>();
    }
}