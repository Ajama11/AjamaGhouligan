using System.Diagnostics.CodeAnalysis;
using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class MyActions
{
    /// If player chooses, don't use this for only grabbing from Hand.
    public static async Task HauntAndPossiblyBury(AjamaGhouliganCard sourceCard, List<PileType> fromPiles, bool andBury = false, bool playerChooses = false, PlayerChoiceContext? choiceContext = null)
    {
        List<CardModel> possibleCards = [];
        List<CardModel> selectedCards;

        foreach (var pile in fromPiles)
        {
            possibleCards = [..possibleCards, ..pile.GetPile(sourceCard.Owner).Cards];
        }

        if (playerChooses)
        {
            ArgumentNullException.ThrowIfNull(choiceContext);

            LocString selectionPrompt =
                andBury ? SepulchrePile.SelectionPrompt.HauntAndBury : SepulchrePile.SelectionPrompt.Haunt;
            
            CardSelectorPrefs prefs = new CardSelectorPrefs(selectionPrompt, sourceCard.DynamicVars.Haunt().IntValue);

            selectedCards = (await CardSelectCmd.FromSimpleGrid(choiceContext, possibleCards, sourceCard.Owner, prefs)).ToList();
        }
        else
        {
            selectedCards = GetRandomCardsFromList(sourceCard.Owner, possibleCards, c => true, sourceCard.DynamicVars.Haunt().IntValue);
        }
        
        foreach (var card in selectedCards)
        {
            if (andBury)
            {
                await HauntAndBurySpecific(card);
            }
            else
            {
                HauntSpecific(card);
            }
        }

        if (selectedCards.Count != 0)
        {
            CardCmd.Preview(selectedCards);
        }
    }
    
    public static async Task HauntAndPossiblyBuryFromHand(AjamaGhouliganCard sourceCard, bool andBury = false, bool playerChooses = false, PlayerChoiceContext? choiceContext = null)
    {
        List<CardModel> possibleCards = PileType.Hand.GetPile(sourceCard.Owner).Cards.ToList();
        List<CardModel> selectedCards;

        if (playerChooses)
        {
            ArgumentNullException.ThrowIfNull(choiceContext);

            LocString selectionPrompt =
                andBury ? SepulchrePile.SelectionPrompt.HauntAndBury : SepulchrePile.SelectionPrompt.Haunt;
            
            CardSelectorPrefs prefs = new CardSelectorPrefs(selectionPrompt, sourceCard.DynamicVars.Haunt().IntValue);

            selectedCards = (await CardSelectCmd.FromHand(choiceContext, sourceCard.Owner, prefs, c => true, sourceCard)).ToList();
        }
        else
        {
            selectedCards = GetRandomCardsFromList(sourceCard.Owner, possibleCards, c => true, sourceCard.DynamicVars.Haunt().IntValue);
        }
        
        foreach (var card in selectedCards)
        {
            if (andBury)
            {
                await HauntAndBurySpecific(card);
            }
            else
            {
                HauntSpecific(card);
            }
        }

        if (selectedCards.Count != 0)
        {
            CardCmd.Preview(selectedCards);
        }
    }
    
    public static void HauntSpecific(CardModel card)
    {
        card.AddKeyword(MyEnums.Haunted);
    }

    public static async Task BurySpecific(CardModel card)
    {
        await CardPileCmd.Add(card, SepulchrePile.PileType);
    }

    public static async Task HauntAndBurySpecific(CardModel card)
    {
        HauntSpecific(card);
        await BurySpecific(card);
    }

    public static async Task Summon(AjamaGhouliganCard sourceCard, PlayerChoiceContext choiceContext)
    {
        await OstyCmd.Summon(choiceContext, sourceCard.Owner, sourceCard.DynamicVars.Summon.BaseValue, sourceCard);
    }

    public static async Task Summon(AbstractModel sourceModel, Player player, int amount, PlayerChoiceContext? choiceContext = null)
    {
        choiceContext ??= new ThrowingPlayerChoiceContext();
        await OstyCmd.Summon(choiceContext, player, amount, sourceModel);
    }
    
    public static async Task<IEnumerable<CardModel>> CreateCards(CardModel canonicalCard, int amount,
        AjamaGhouliganCard sourceCard, PileType pile = PileType.Hand, CardPilePosition position = CardPilePosition.Bottom)
    {
        return await CreateCards(canonicalCard, amount, sourceCard.Owner, sourceCard.CombatState!, pile, position);
    }
    
    public static async Task<IEnumerable<CardModel>> CreateCards(CardModel canonicalCard, int amount, Player owner, ICombatState combatState, PileType pile = PileType.Hand, CardPilePosition position = CardPilePosition.Bottom)
    {
        if (amount == 0 || CombatManager.Instance.IsOverOrEnding)
        {
            return [];
        }

        List<CardModel> cards = [];

        for (int i = 0; i < amount; i++)
        {
            cards.Add(combatState.CreateCard(canonicalCard, owner));
        }

        IReadOnlyList<CardPileAddResult> results = await CardPileCmd.AddGeneratedCardsToCombat(cards, pile, owner, position);

        if (pile != PileType.Hand) CardCmd.PreviewCardPileAdd(results);

        return cards;
    }
    
    public static async Task PutSelect(AjamaGhouliganCard sourceCard, PlayerChoiceContext choiceContext, PileType from, PileType to, LocString selectionScreenPrompt,
        CardPilePosition position = CardPilePosition.Bottom, int amount = 1)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(selectionScreenPrompt, amount);

        CardModel? card = (await CardSelectCmd.FromSimpleGrid(choiceContext, from.GetPile(sourceCard.Owner).Cards, sourceCard.Owner, prefs)).FirstOrDefault();

        if (card != null) await CardPileCmd.Add(card, to, position);
    }
    
    public static async Task PutSelectFiltered(AjamaGhouliganCard sourceCard, PlayerChoiceContext choiceContext, PileType from, PileType to, LocString selectionScreenPrompt, Func<CardModel, bool> filter,
        CardPilePosition position = CardPilePosition.Bottom, int amount = 1)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(selectionScreenPrompt, amount);

        CardModel? card = (await CardSelectCmd.FromSimpleGrid(choiceContext, from.GetPile(sourceCard.Owner).Cards.Where(filter).ToList(), sourceCard.Owner, prefs)).FirstOrDefault();

        if (card != null) await CardPileCmd.Add(card, to, position);
    }

    public static List<CardModel> GetRandomCards(AjamaGhouliganCard sourceCard, PileType from, Func<CardModel, bool> filter, int amount = 1)
    {
        return GetRandomCards(sourceCard.Owner, from, filter, amount);
    }
    
    public static List<CardModel> GetRandomCards(Player player, PileType from, Func<CardModel, bool> filter, int amount = 1)
    {
        List<CardModel> cards = from.GetPile(player).Cards
            .Where(filter)
            .TakeRandom(amount, player.RunState.Rng.CombatCardSelection)
            .ToList();
        
        return cards;
    }
    
    public static List<CardModel> GetRandomCardsFromList(Player player, List<CardModel> cards, Func<CardModel, bool> filter, int amount = 1)
    {
        List<CardModel> selectedCards = cards
            .Where(filter)
            .TakeRandom(amount, player.RunState.Rng.CombatCardSelection)
            .ToList();
        
        return selectedCards;
    }
}