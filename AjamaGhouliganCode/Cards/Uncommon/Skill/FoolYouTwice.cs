using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Uncommon.Skill;

public class FoolYouTwice() : AjamaGhouliganCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        List<CardModel> allSurprises = Owner.PlayerCombatState!.AllCards.Where(
                c => c is Surprise && c.Pile != null)
            .ToList();
        
        List<CardModel> surprisesNotAlreadyInDrawPile = allSurprises.Where(
                c => c.Pile!.Type != PileType.Draw)
            .ToList();

        if (IsUpgraded)
        {
            CardCmd.Upgrade(allSurprises, CardPreviewStyle.HorizontalLayout);
        }

        await CardPileCmd.Add(surprisesNotAlreadyInDrawPile, PileType.Draw, CardPilePosition.Random);
    }
}