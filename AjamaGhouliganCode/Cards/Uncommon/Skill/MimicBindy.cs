using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Uncommon.Skill;

public class MimicBindy() : AjamaGhouliganCard(1,
    CardType.Skill, CardRarity.Uncommon,
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
        List<CardModel> list = CardFactory.GetDistinctForCombat(Owner,
            ModelDb.CardPool<NecrobinderCardPool>()
                .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                .Where(c => c.Type == CardType.Power), 3,
            Owner.RunState.Rng.CombatCardGeneration).ToList();

        if (IsUpgraded)
        {
            foreach (CardModel possibleCard in list)
            {
                CardCmd.Upgrade(possibleCard);
            }
        }

        CardModel? card = await CardSelectCmd.FromChooseACardScreen(choiceContext, list, Owner, true);
        if (card == null) return;
        
        card.SetToFreeThisTurn();
        
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }
}