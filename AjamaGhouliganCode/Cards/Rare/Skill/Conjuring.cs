using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Rare.Skill;

public class Conjuring() : AjamaGhouliganCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.None)
{
    protected override bool HasEnergyCostX => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        MyEnums.Haunted
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int xValue = ResolveEnergyXValue();
        if (IsUpgraded) ++xValue;
        if (xValue == 0) return;

        List<CardModel> cards = (await CommonActions.SelectCards(
            this, 
            new CardSelectorPrefs(SelectionScreenPrompt, 0, xValue),
            choiceContext, SepulchrePile.PileType))
            .ToList();

        foreach (CardModel card in cards)
        {
            await CardCmd.AutoPlay(choiceContext, card.CreateDupe(Owner), null);
        }
        
        if (cards.Count > 0)
        {
            await CardCmd.Exhaust(choiceContext, this);
        }
    }
}