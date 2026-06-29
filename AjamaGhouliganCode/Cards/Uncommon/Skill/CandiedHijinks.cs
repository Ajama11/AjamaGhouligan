using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Token.Treats;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Uncommon.Skill;

public class CandiedHijinks() : AjamaGhouliganCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SurpriseVar(2),
        new TreatVar(2, true)
    ];
    
    public override IEnumerable<IHoverTip> MyHoverTips =>
    [
        HoverTipFactory.FromCard<HomemadeCookie>(), // Combining cycling and normal previews doesn't work right now
        HoverTipFactory.FromCard<Surprise>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        if (WasTreatPlayedThisTurn)
        {
            await MyActions.CreateSurprises(this);
        }

        if (WasSurprisePlayedThisTurn)
        {
            await MyActions.CreateTreats(this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Surprise.UpgradeValueBy(1);
    }

    protected override bool ShouldGlowGoldInternal => 
        WasTreatPlayedThisTurn || WasSurprisePlayedThisTurn;

    private bool WasTreatPlayedThisTurn =>
        CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>().Any(e =>
            e.HappenedThisTurn(CombatState) && 
            e.Actor == Owner.Creature && 
            e.CardPlay.Card.Tags.Contains(MyEnums.Treat));
    
    private bool WasSurprisePlayedThisTurn =>
        CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>().Any(e =>
            e.HappenedThisTurn(CombatState) && 
            e.Actor == Owner.Creature && 
            e.CardPlay.Card is Surprise);
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("TreatPlayed", WasTreatPlayedThisTurn || !IsInCombat);
        description.Add("SurprisePlayed", WasSurprisePlayedThisTurn || !IsInCombat);
    }
}