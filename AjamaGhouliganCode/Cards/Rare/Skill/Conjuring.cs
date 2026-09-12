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
    
    public static readonly SpireField<CardModel, bool> SelectedByAnotherConjuring =
        new SpireField<CardModel, bool>(() => false).CopyOnClone();

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
            choiceContext, SepulchrePile.PileType, c => c != DupeOf && !SelectedByAnotherConjuring[c]))
            .ToList();

        foreach (var conjuring in cards.OfType<Conjuring>())
        {
            SelectedByAnotherConjuring[conjuring] = true;
        }
        
        foreach (CardModel card in cards)
        {
            await CardCmd.AutoPlay(choiceContext, card.CreateDupe(Owner), null);
        }

        if (!SelectedByAnotherConjuring[this])
        {
            foreach (var card in Owner.PlayerCombatState!.AllCards)
            {
                SelectedByAnotherConjuring[card] = false;
            }
        }
        
        if (cards.Count > 0 && !IsDupe)
        {
            await CardCmd.Exhaust(choiceContext, this);
        }
    }
}