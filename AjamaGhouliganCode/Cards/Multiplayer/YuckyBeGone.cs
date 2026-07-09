using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Multiplayer;

public class YuckyBeGone() : AjamaGhouliganCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    private const string SurpriseAmount = "SurpriseAmount";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        ..MakeCalculatedVar(SurpriseAmount, 0,
            static (card, _) =>
            {
                return card.CombatState!.GetTeammatesOf(card.Owner.Creature)
                    .Where(c => 
                        c is { IsAlive: true, IsPlayer: true } && 
                        c != card.Owner.Creature)
                    .Sum(creature => GetStatusAndCurseCards(creature.Player!).Count);
            })
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    public override IEnumerable<IHoverTip> MyHoverTips =>
    [
        HoverTipFactory.FromCard<Surprise>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        foreach (Creature creature in CombatState!
                     .GetTeammatesOf(Owner.Creature)
                     .Where(c =>
                         c is { IsAlive: true, IsPlayer: true } &&
                         c != Owner.Creature))
        {
            foreach (CardModel card in GetStatusAndCurseCards(creature.Player!))
            {
                await CardCmd.TransformTo<Surprise>(card);
                await MyActions.CreateSurprises(DynamicVars[SurpriseAmount + "Extra"].IntValue, Owner, CombatState);
            }
        }
    }

    private static List<CardModel> GetStatusAndCurseCards(Player player)
    {
        return player.PlayerCombatState!.AllCards
            .Where(c =>
                c.Pile != null &&
                c.Pile.Type != PileType.Exhaust &&
                c.Pile.Type != PileType.Play &&
                c.Type is CardType.Status or CardType.Curse)
            .ToList();
    }

    protected override void OnUpgrade()
    {
        DynamicVars[SurpriseAmount + "Extra"].UpgradeValueBy(1);
    }
}