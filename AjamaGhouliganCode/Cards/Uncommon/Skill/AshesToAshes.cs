using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Uncommon.Skill;

public class AshesToAshes() : AjamaGhouliganCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new LoseDoomVar(6)
    ];

    public override IEnumerable<IHoverTip> MyHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        List<CardModel> bothPiles = [..PileType.Draw.GetPile(Owner).Cards, ..PileType.Discard.GetPile(Owner).Cards];
        
        List<CardModel> eligibleCards = bothPiles
            .Where(c => 
                !c.Keywords.Contains(CardKeyword.Ethereal) &&
                !c.Keywords.Contains(CardKeyword.Retain))
            .ToList();

        List<CardModel> chosenCards = MyActions.GetRandomCardsFromList(Owner, eligibleCards, _ => true, DynamicVars.Cards.IntValue);
        
        if (chosenCards.Count < DynamicVars.Cards.IntValue)
        {
            List<CardModel> snapshottedCards = [..chosenCards];

            chosenCards =
            [
                ..snapshottedCards,
                ..MyActions.GetRandomCardsFromList(Owner, bothPiles,
                    c => 
                        !c.Keywords.Contains(CardKeyword.Ethereal) && 
                        c.Keywords.Contains(CardKeyword.Retain),
                    DynamicVars.Cards.IntValue - snapshottedCards.Count)
            ];
        }

        foreach (CardModel card in chosenCards)
        {
            card.AddKeyword(CardKeyword.Ethereal);
            CardCmd.Preview(card, 2f);
        }

        await MyActions.LoseDoom(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.LoseDoom.UpgradeValueBy(3);
    }
}