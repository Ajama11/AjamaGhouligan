using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Uncommon.Skill;

public class CrowsGift() : AjamaGhouliganCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    { 
        List<CardModel> colorlessCards = CardFactory.GetDistinctForCombat(Owner,
                ModelDb.CardPool<ColorlessCardPool>()
                    .GetUnlockedCards(
                        Owner.UnlockState,
                        Owner.RunState.CardMultiplayerConstraint),
                DynamicVars.Cards.IntValue,
                Owner.RunState.Rng.CombatCardGeneration)
            .ToList();
        
        foreach (CardModel card in colorlessCards)
        {
            card.SetToFreeThisCombat();
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, Owner, CardPilePosition.Top));
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}