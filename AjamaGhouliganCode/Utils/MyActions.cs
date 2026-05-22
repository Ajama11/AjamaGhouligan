using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class MyActions
{
    /// If player chooses, don't use this for only grabbing from Hand.
    public static async Task HauntAndPossiblyBury(AjamaGhouliganCard sourceCard, List<PileType> fromPiles, bool andBury = false, bool playerChooses = false, PlayerChoiceContext? choiceContext = null, Func<CardModel, bool>? filter = null)
    {
        List<CardModel> possibleCards = [];
        List<CardModel> selectedCards;

        foreach (var pile in fromPiles)
        {
            possibleCards = [..possibleCards, ..pile.GetPile(sourceCard.Owner).Cards];
        }

        if (filter != null)
        {
            possibleCards = possibleCards.Where(filter).ToList();
        }

        if (possibleCards.Count == 0)
        {
            return;
        }

        if (playerChooses)
        {
            ArgumentNullException.ThrowIfNull(choiceContext);

            LocString selectionPrompt =
                andBury ? MySelectionPrompts.HauntAndBury : MySelectionPrompts.Haunt;
            
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
    }
    
    public static async Task HauntAndPossiblyBuryFromHand(AjamaGhouliganCard sourceCard, bool andBury = false, bool playerChooses = false, PlayerChoiceContext? choiceContext = null, Func<CardModel, bool>? filter = null)
    {
        List<CardModel> possibleCards = PileType.Hand.GetPile(sourceCard.Owner).Cards.ToList();
        List<CardModel> selectedCards;
        
        if (filter != null)
        {
            possibleCards = possibleCards.Where(filter).ToList();
        }
        
        if (possibleCards.Count == 0)
        {
            return;
        }

        if (playerChooses)
        {
            ArgumentNullException.ThrowIfNull(choiceContext);

            LocString selectionPrompt =
                andBury ? MySelectionPrompts.HauntAndBury : MySelectionPrompts.Haunt;
            
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
    }
    
    public static void HauntSpecific(CardModel card, bool preview = true)
    {
        if (card.Keywords.Contains(CardKeyword.Unplayable)) return;
        
        card.AddKeyword(MyEnums.Haunted);
        if (preview && card.Pile!.Type != PileType.Hand) CardCmd.Preview(card);
    }
    
    public static void HauntSpecific(List<CardModel> cards, bool preview = true)
    {
        List<CardModel> cardsToPreview = [];
        
        foreach (CardModel card in cards)
        {
            if (card.Keywords.Contains(CardKeyword.Unplayable)) continue;
            
            card.AddKeyword(MyEnums.Haunted);
            if (preview && card.Pile!.Type != PileType.Hand)
                cardsToPreview = [..cardsToPreview, card];
        }
        
        if (cardsToPreview.Count > 0) CardCmd.Preview(cardsToPreview);
    }

    public static async Task BurySpecific(CardModel card)
    {
        if (card.Pile!.Type == PileType.Hand)
        {
            await CardPileCmd.Add(card, SepulchrePile.PileType);
            return;
        }

        NCombatCardPile pile;

        switch (card.Pile.Type)
        {
            case PileType.Draw:
                pile = NCombatRoom.Instance!.Ui.DrawPile;
                break;
            case PileType.Discard:
                pile = NCombatRoom.Instance!.Ui.DiscardPile;
                break;
            case PileType.None:
            case PileType.Hand:
            case PileType.Exhaust:
            case PileType.Play:
            case PileType.Deck:
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, SepulchrePile.PileType, CardPilePosition.Bottom, null, true));
        
        if (pile._pile == null) return;
        pile._currentCount = pile._pile!.Cards.Count;
        pile._countLabel.SetTextAutoSize(pile._currentCount.ToString());
        pile._countLabel.PivotOffset = pile._countLabel.Size * 0.5F;
        
        foreach (var model in card.CombatState!.IterateHookListeners())
        {
            if (model is not IOnBury onBuryModel) continue;
            await onBuryModel.OnBury(card);
            model.InvokeExecutionFinished();
        }
    }
    
    public static async Task BurySpecific(List<CardModel> cards)
    {
        if (cards.Count == 0) return;

        List<CardModel> cardsInHand = cards.Where(c => c.Pile!.Type == PileType.Hand).ToList();
        List<CardModel> cardsOutsideHand = cards.Where(c => c.Pile!.Type != PileType.Hand).ToList();

        bool drawPileAffected = cardsOutsideHand.Any(c => c.Pile!.Type == PileType.Draw);
        bool discardPileAffected = cardsOutsideHand.Any(c => c.Pile!.Type == PileType.Discard);

        await CardPileCmd.Add(cardsInHand, SepulchrePile.PileType);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(cardsOutsideHand, SepulchrePile.PileType, CardPilePosition.Bottom, null, true));
        
        if (drawPileAffected)
        {
            NCombatCardPile pile = NCombatRoom.Instance!.Ui.DrawPile;
            if (pile._pile == null) return;

            pile._currentCount = pile._pile!.Cards.Count;
            pile._countLabel.SetTextAutoSize(pile._currentCount.ToString());
            pile._countLabel.PivotOffset = pile._countLabel.Size * 0.5F;
        }
        
        if (discardPileAffected)
        {
            NCombatCardPile pile = NCombatRoom.Instance!.Ui.DiscardPile;
            if (pile._pile == null) return;

            pile._currentCount = pile._pile!.Cards.Count;
            pile._countLabel.SetTextAutoSize(pile._currentCount.ToString());
            pile._countLabel.PivotOffset = pile._countLabel.Size * 0.5F;
        }
        
        foreach (var model in cards.First().CombatState!.IterateHookListeners())
        {
            MainFile.Logger.Info("AAAAAA");
            if (model is not IOnBury onBuryModel) continue;

            foreach (CardModel card in cards)
            {
                MainFile.Logger.Info(nameof(card));
                await onBuryModel.OnBury(card);
            }
            
            model.InvokeExecutionFinished();
        }
    }

    public static async Task HauntAndBurySpecific(CardModel card)
    {
        HauntSpecific(card, false);
        await BurySpecific(card);
    }
    
    public static async Task HauntAndBurySpecific(List<CardModel> cards)
    {
        HauntSpecific(cards, false);
        await BurySpecific(cards);
    }

    public static async Task Summon(PlayerChoiceContext choiceContext, AjamaGhouliganCard sourceCard)
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
    
    public static async Task PutSelect(PlayerChoiceContext choiceContext, AjamaGhouliganCard sourceCard, PileType from, PileType to, LocString selectionScreenPrompt,
        CardPilePosition position = CardPilePosition.Bottom, int amount = 1)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(selectionScreenPrompt, amount);

        List<CardModel> cards = (await CardSelectCmd.FromSimpleGrid(choiceContext, from.GetPile(sourceCard.Owner).Cards, sourceCard.Owner, prefs)).ToList();

        if (cards.Count != 0) await CardPileCmd.Add(cards, to, position);
    }
    
    public static async Task PutSelectFiltered(AjamaGhouliganCard sourceCard, PlayerChoiceContext choiceContext, PileType from, PileType to, LocString selectionScreenPrompt, Func<CardModel, bool> filter,
        CardPilePosition position = CardPilePosition.Bottom, int amount = 1)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(selectionScreenPrompt, amount);

        List<CardModel> cards = (await CardSelectCmd.FromSimpleGrid(choiceContext, from.GetPile(sourceCard.Owner).Cards.Where(filter).ToList(), sourceCard.Owner, prefs)).ToList();

        if (cards.Count != 0) await CardPileCmd.Add(cards, to, position);
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
    
    public static async Task SelectForBury(PlayerChoiceContext choiceContext, AjamaGhouliganCard sourceCard, PileType from = PileType.Hand, int amountOverride = -1)
    {
        int amount = amountOverride == -1 ? sourceCard.DynamicVars.Bury().IntValue : amountOverride;
        
        CardSelectorPrefs prefs = new CardSelectorPrefs(MySelectionPrompts.Bury, amount)
        {
            ShouldGlowGold = c => c.Keywords.Contains(MyEnums.Haunted)
        };

        List<CardModel> cards;

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (from == PileType.Hand)
        {
            cards = (await CardSelectCmd.FromHand(choiceContext, sourceCard.Owner, prefs, _ => true, sourceCard)).ToList();
        }
        else
        {
            cards = (await CardSelectCmd.FromSimpleGrid(choiceContext, from.GetPile(sourceCard.Owner).Cards, sourceCard.Owner, prefs)).ToList();
        }

        await BurySpecific(cards);
    }

    public static async Task SelfDoom(PlayerChoiceContext choiceContext, AjamaGhouliganCard sourceCard)
    {
        await SelfDoom(sourceCard.Owner.Creature, sourceCard.DynamicVars.Power<DoomPower>().IntValue, choiceContext, sourceCard);
    }

    public static async Task SelfDoom(Creature playerCreature, int amount, PlayerChoiceContext? choiceContext = null, CardModel? sourceCard = null)
    {
        choiceContext ??= new ThrowingPlayerChoiceContext();

        await PowerCmd.Apply<DoomPower>(choiceContext, playerCreature, amount, playerCreature, sourceCard);
    }
    
    public static async Task LoseDoom(PlayerChoiceContext choiceContext, AjamaGhouliganCard sourceCard)
    {
        await LoseDoom(sourceCard.Owner.Creature, sourceCard.DynamicVars.LoseDoom().IntValue, choiceContext, sourceCard);
    }

    public static async Task LoseDoom(Creature playerCreature, int amount, PlayerChoiceContext? choiceContext = null, CardModel? sourceCard = null)
    {
        choiceContext ??= new ThrowingPlayerChoiceContext();
        
        DoomPower? doom = playerCreature.GetPower<DoomPower>();
        if (doom == null) return;
        
        await PowerCmd.ModifyAmount(choiceContext, doom, -1 * amount, playerCreature, sourceCard);
    }

    public static void HauntRandomInPile(PileType pile, AjamaGhouliganCard sourceCard)
    {
        HauntRandomInPile(pile, sourceCard.Owner, sourceCard.DynamicVars.Haunt().IntValue);
    }
    
    public static void HauntRandomInPile(PileType pile, Player player, int amount)
    {
        List<CardModel> chosenCards = GetRandomCards(player, pile,
            c => !c.Keywords.Contains(MyEnums.Haunted) && !c.Keywords.Contains(CardKeyword.Unplayable), amount);

        foreach (CardModel card in chosenCards)
        {
            HauntSpecific(card);
        }
    }
    
    public static async Task BuryRandomInPile(PileType pile, AjamaGhouliganCard sourceCard, MyEnums.RandomBuryTargeting targeting = MyEnums.RandomBuryTargeting.All)
    {
        await BuryRandomInPile(pile, sourceCard.Owner, sourceCard.DynamicVars.Bury().IntValue, targeting);
    }
    
    public static async Task BuryRandomInPile(PileType pile, Player player, int amount, MyEnums.RandomBuryTargeting targeting = MyEnums.RandomBuryTargeting.All)
    {
        Func<CardModel, bool> filter = targeting switch
        {
            MyEnums.RandomBuryTargeting.All => 
                _ => true,
            
            MyEnums.RandomBuryTargeting.NotHaunted => 
                c => !c.Keywords.Contains(MyEnums.Haunted),
            
            MyEnums.RandomBuryTargeting.PrioritizeHaunted or MyEnums.RandomBuryTargeting.OnlyHaunted => 
                c => c.Keywords.Contains(MyEnums.Haunted),
            
            _ => throw new ArgumentOutOfRangeException(nameof(targeting), targeting, null)
        };

        List<CardModel> chosenCards = GetRandomCards(player, pile, filter, amount);

        if (chosenCards.Count < amount && targeting == MyEnums.RandomBuryTargeting.PrioritizeHaunted)
        {
            List<CardModel> snapshottedChosenCards = [..chosenCards];
            
            chosenCards = [..chosenCards, ..GetRandomCards(player, pile, c => !snapshottedChosenCards.Contains(c), amount - chosenCards.Count)];
        }

        await BurySpecific(chosenCards);
    }

    public static async Task Goof(PlayerChoiceContext choiceContext, CardModel sourceCard)
    {
        await CommonActions.ApplySelf<GoofPower>(choiceContext, sourceCard);
    }

    public static async Task Misfortune(PlayerChoiceContext choiceContext, Creature target, DynamicVarSource dynVarSource)
    {
        await CommonActions.Apply<MisfortunePower>(choiceContext, target, dynVarSource);
    }
    
    public static async Task Misfortune(PlayerChoiceContext choiceContext, IEnumerable<Creature> targets, DynamicVarSource dynVarSource)
    {
        await CommonActions.Apply<MisfortunePower>(choiceContext, targets, dynVarSource);
    }

    public static async Task OstyHeal(AjamaGhouliganCard sourceCard)
    {
        await OstyHeal(sourceCard.Owner, sourceCard.DynamicVars.Heal.BaseValue);
    }
    
    public static async Task OstyHeal(Player player, decimal amount)
    {
        await CreatureCmd.Heal(player.Osty!, amount);
    }

    public static async Task Disinter(PlayerChoiceContext choiceContext, AjamaGhouliganCard sourceCard)
    {
        await PutSelect(choiceContext, sourceCard, SepulchrePile.PileType, PileType.Hand, MySelectionPrompts.Disinter, CardPilePosition.Bottom, sourceCard.DynamicVars.Disinter().IntValue);
    }

    public static void GainsHauntedAndBury(CardModel card, bool preview = true)
    {
        HauntSpecific(card, preview);
        
        if (!card.Keywords.Contains(CardKeyword.Exhaust))
            card.AddKeyword(MyEnums.Bury);
    }

    public static void GainsHauntedAndBury(List<CardModel> cards, bool preview = true)
    {
        HauntSpecific(cards, preview);

        foreach (CardModel card in cards)
        {
            if (!card.Keywords.Contains(CardKeyword.Exhaust) && !card.Keywords.Contains(CardKeyword.Unplayable))
                card.AddKeyword(MyEnums.Bury);
        }
    }
}