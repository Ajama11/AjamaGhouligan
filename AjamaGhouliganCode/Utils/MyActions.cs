using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Status;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Token.Treats;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Extensions;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
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
            
            CardSelectorPrefs prefs = new CardSelectorPrefs(selectionPrompt, sourceCard.DynamicVars.Haunt.IntValue);

            selectedCards = (await CardSelectCmd.FromSimpleGrid(choiceContext, possibleCards, sourceCard.Owner, prefs)).ToList();
        }
        else
        {
            selectedCards = GetRandomCardsFromList(sourceCard.Owner, possibleCards, c => true, sourceCard.DynamicVars.Haunt.IntValue);
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
            
            CardSelectorPrefs prefs = new CardSelectorPrefs(selectionPrompt, sourceCard.DynamicVars.Haunt.IntValue);

            selectedCards = (await CardSelectCmd.FromHand(choiceContext, sourceCard.Owner, prefs, c => true, sourceCard)).ToList();
        }
        else
        {
            selectedCards = GetRandomCardsFromList(sourceCard.Owner, possibleCards, c => true, sourceCard.DynamicVars.Haunt.IntValue);
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

        NCombatCardPile? pile = null;

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
                break;
        }
        
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, SepulchrePile.PileType, CardPilePosition.Bottom, null, true));

        if (pile?._pile == null) return;
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
            if (model is not IOnBury onBuryModel) continue;

            foreach (CardModel card in cards)
            {
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
    
    public static async Task HalfSummon(PlayerChoiceContext choiceContext, AjamaGhouliganCard sourceCard)
    {
        await HalfSummon(sourceCard, sourceCard.Owner,
            sourceCard.DynamicVars.HalfSummonFilled.IntValue, sourceCard.DynamicVars.HalfSummonEmpty.IntValue,
            choiceContext);
    }

    public static async Task<SummonResult> HalfSummon(AbstractModel sourceModel, Player summoner, int filledValue, int emptyValue, PlayerChoiceContext? choiceContext)
    {
        choiceContext ??= new ThrowingPlayerChoiceContext();
        ICombatState combatState = summoner.Creature.CombatState!;
        
        filledValue = (int) Hook.ModifySummonAmount(combatState, summoner, filledValue, sourceModel);
        emptyValue = (int) Hook.ModifySummonAmount(combatState, summoner, emptyValue, sourceModel);

        decimal totalValue = filledValue + emptyValue;
        
        if (totalValue == 0M) return new SummonResult(summoner.Osty, 0M);
        
        if (CombatManager.Instance.IsInProgress) 
            SfxCmd.Play("event:/sfx/characters/necrobinder/necrobinder_summon");
        
        Creature? osty = combatState.Allies.FirstOrDefault(c => c.Monster is Osty && c.PetOwner == summoner);
        
        if (summoner.IsOstyAlive)
        {
            await CreatureCmd.SetMaxHp(osty!, osty!.MaxHp + totalValue);
            await CreatureCmd.Heal(osty, filledValue);
        }
        else
        { 
            bool isReviving = osty != null;
            
            if (isReviving) 
            { 
                if (osty!.IsAlive) 
                    throw new InvalidOperationException("We shouldn't make it here if Osty is still alive!"); 
                summoner.PlayerCombatState!.AddPetInternal(osty); 
            }
            else 
            { 
                osty = await PlayerCmd.AddPet<Osty>(summoner); 
                NCreature? ostyNode = NCombatRoom.Instance?.GetCreatureNode(osty);
                
                if (ostyNode != null && sourceModel is CardModel) 
                { 
                    ostyNode.Modulate = Colors.Transparent; 
                    ostyNode.CreateTween().TweenProperty(ostyNode, (NodePath) "modulate", Colors.White, 0.3499999940395355).SetDelay(0.10000000149011612); 
                    ostyNode.StartReviveAnim(); 
                }
                
                await PowerCmd.Apply<DieForYouPower>(choiceContext, osty, 1, null, null);
                
                ostyNode?.TrackBlockStatus(summoner.Creature); 
            }
            
            await CreatureCmd.SetMaxHp(osty, totalValue);
            await CreatureCmd.Heal(osty, filledValue, isReviving);
            
            if (isReviving) await Hook.AfterOstyRevived(combatState, osty);
        }
        
        NCombatRoom.Instance?.GetCreatureNode(osty)?.OstyScaleToSize(osty.MaxHp, 0.75);
        
        CombatManager.Instance.History.Summoned(combatState, filledValue, summoner);
        
        await Hook.AfterSummon(combatState, choiceContext, summoner, filledValue);
        
        return new SummonResult(summoner.Osty, filledValue);
    }
    
    public static async Task<IEnumerable<CardModel>> CreateCards(CardModel canonicalCard, int amount,
        AjamaGhouliganCard sourceCard, PileType pile = PileType.Hand, CardPilePosition position = CardPilePosition.Bottom)
    {
        return await CreateCards(canonicalCard, amount, sourceCard.Owner, sourceCard.CombatState!, pile, position);
    }
    
    public static async Task<IEnumerable<CardModel>> CreateCards(CardModel canonicalCard, int amount, Player owner, ICombatState combatState, PileType pile = PileType.Hand, CardPilePosition position = CardPilePosition.Bottom, bool preview = true, float previewTime = 1.2f, Func<List<CardModel>, List<CardModel>>? modifyCardsBeforePreview = null)
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

        if (modifyCardsBeforePreview != null)
        {
            cards = modifyCardsBeforePreview(cards);
        }

        IReadOnlyList<CardPileAddResult> results = await CardPileCmd.AddGeneratedCardsToCombat(cards, pile, owner, position);

        if (pile != PileType.Hand && preview) CardCmd.PreviewCardPileAdd(results, previewTime);

        return cards;
    }
    
    public static async Task<List<CardModel>> PutSelect(PlayerChoiceContext choiceContext, AjamaGhouliganCard sourceCard, PileType from, PileType to, LocString selectionScreenPrompt,
        CardPilePosition position = CardPilePosition.Bottom, int amount = 1, bool upTo = false)
    {
        CardSelectorPrefs prefs = upTo ? 
            new CardSelectorPrefs(selectionScreenPrompt, 0, amount) :
            new CardSelectorPrefs(selectionScreenPrompt, amount);

        List<CardModel> cards = (await CardSelectCmd.FromSimpleGrid(choiceContext, from.GetPile(sourceCard.Owner).Cards, sourceCard.Owner, prefs)).ToList();

        if (cards.Count != 0) await CardPileCmd.Add(cards, to, position);

        return cards;
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
    
    public static async Task SelectForBury(PlayerChoiceContext choiceContext, AjamaGhouliganCard sourceCard, PileType from = PileType.Hand, bool upTo = false, int amountOverride = -1)
    {
        int amount = amountOverride == -1 ? sourceCard.DynamicVars.Bury.IntValue : amountOverride;

        await SelectForBury(sourceCard, choiceContext, sourceCard.Owner, amount, from, upTo);
    }
    
    public static async Task SelectForBury(AbstractModel sourceModel, PlayerChoiceContext choiceContext, Player player, int amount, PileType from = PileType.Hand, bool upTo = false)
    {
        CardSelectorPrefs prefs = upTo ?
            new CardSelectorPrefs(MySelectionPrompts.BuryUpTo, 0, amount) :
            new CardSelectorPrefs(MySelectionPrompts.Bury, amount);
        prefs.ShouldGlowGold = c => c.Keywords.Contains(MyEnums.Haunted);

        List<CardModel> cards;

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (from == PileType.Hand)
        {
            cards = (await CardSelectCmd.FromHand(choiceContext, player, prefs, _ => true, sourceModel)).ToList();
        }
        else
        {
            cards = (await CardSelectCmd.FromSimpleGrid(choiceContext, from.GetPile(player).Cards, player, prefs)).ToList();
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
        await LoseDoom(sourceCard.Owner.Creature, sourceCard.DynamicVars.LoseDoom.IntValue, choiceContext, sourceCard);
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
        HauntRandomInPile(pile, sourceCard.Owner, sourceCard.DynamicVars.Haunt.IntValue);
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
    
    public static async Task BuryRandomInPile(PileType pile, AjamaGhouliganCard sourceCard, MyEnums.RandomBuryTargeting targeting = MyEnums.RandomBuryTargeting.All, Func<CardModel, bool>? filter = null)
    {
        await BuryRandomInPile(pile, sourceCard.Owner, sourceCard.DynamicVars.Bury.IntValue, targeting, filter);
    }
    
    public static async Task BuryRandomInPile(PileType pile, Player player, int amount, MyEnums.RandomBuryTargeting targeting = MyEnums.RandomBuryTargeting.All, Func<CardModel, bool>? filter = null)
    {
        await BuryRandomInPiles([pile], player, amount, targeting, filter);
    }
    
    public static async Task BuryRandomInPiles(List<PileType> piles, AjamaGhouliganCard sourceCard, MyEnums.RandomBuryTargeting targeting = MyEnums.RandomBuryTargeting.All, Func<CardModel, bool>? filter = null)
    {
        await BuryRandomInPiles(piles, sourceCard.Owner, sourceCard.DynamicVars.Bury.IntValue, targeting, filter);
    }
    
    public static async Task BuryRandomInPiles(List<PileType> piles, Player player, int amount, MyEnums.RandomBuryTargeting targeting = MyEnums.RandomBuryTargeting.All, Func<CardModel, bool>? filter = null)
    {
        filter ??= _ => true;
        
        Func<CardModel, bool> combinedFilter = targeting switch
        {
            MyEnums.RandomBuryTargeting.All => 
                filter,
            
            MyEnums.RandomBuryTargeting.NotHaunted => 
                c => !c.Keywords.Contains(MyEnums.Haunted) && filter(c),
            
            MyEnums.RandomBuryTargeting.PrioritizeHaunted or MyEnums.RandomBuryTargeting.OnlyHaunted => 
                c => c.Keywords.Contains(MyEnums.Haunted) && filter(c),
            
            _ => throw new ArgumentOutOfRangeException(nameof(targeting), targeting, null)
        };

        List<CardModel> cardsInPiles = [];

        foreach (PileType pile in piles)
        {
            cardsInPiles = [..cardsInPiles, ..pile.GetPile(player).Cards];
        }

        List<CardModel> chosenCards = GetRandomCardsFromList(player, cardsInPiles, combinedFilter, amount);

        if (chosenCards.Count < amount && targeting == MyEnums.RandomBuryTargeting.PrioritizeHaunted)
        {
            List<CardModel> snapshottedChosenCards = [..chosenCards];
            
            chosenCards = [..chosenCards, ..GetRandomCardsFromList(player, cardsInPiles, c => !snapshottedChosenCards.Contains(c), amount - chosenCards.Count)];
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
        if (Osty.IsReadyToParty(player)) await CreatureCmd.Heal(player.Osty!, amount);
    }

    public static async Task DisinterSelect(PlayerChoiceContext choiceContext, AjamaGhouliganCard sourceCard, bool upTo = false)
    {
        List<CardModel> disinterredCards = await PutSelect(choiceContext, sourceCard, 
            SepulchrePile.PileType, PileType.Hand, 
            MySelectionPrompts.Disinter, 
            CardPilePosition.Bottom, 
            sourceCard.DynamicVars.Disinter.IntValue, 
            upTo);

        foreach (var card in disinterredCards)
        {
            await HandleDisinter(card);
        }
    }

    public static async Task DisinterRandomNonHaunted(AjamaGhouliganCard sourceCard)
    {
        List<CardModel> disinterredCards = GetRandomCards(sourceCard,
            SepulchrePile.PileType,
            c => !c.Keywords.Contains(MyEnums.Haunted),
            sourceCard.DynamicVars.Disinter.IntValue);

        await CardPileCmd.Add(disinterredCards, PileType.Hand);
        
        foreach (var card in disinterredCards)
        {
            await HandleDisinter(card);
        }
    }

    private static async Task HandleDisinter(CardModel card)
    {
        SepulchreSingleton.RemoveFromCurrentAutoplay.Set(card, true);
        
        foreach (var model in card.CombatState!.IterateHookListeners())
        {
            if (model is not IOnDisinter onDisinterModel) continue;
            await onDisinterModel.OnDisinter(card);
            model.InvokeExecutionFinished();
        }
    }

    public static void GainsHauntedAndBury(CardModel card, bool preview = true)
    {
        GainsHauntedAndBury([card], preview);
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
    
    public static void GainsBury(CardModel card, bool preview = true)
    {
        GainsBury([card], preview);
    }
    
    public static void GainsBury(List<CardModel> cards, bool preview = true)
    { 
        foreach (CardModel card in cards)
        {
            if (!card.Keywords.Contains(CardKeyword.Exhaust) && !card.Keywords.Contains(CardKeyword.Unplayable))
            {
                card.AddKeyword(MyEnums.Bury);
            }
        }
        
        if (preview) CardCmd.Preview(cards);
    }

    public static readonly List<CardModel> CanonicalTreats =
    [
        ModelDb.Card<Bubblegum>(),
        ModelDb.Card<GummyWorm>(),
        ModelDb.Card<HomemadeCookie>(),
        ModelDb.Card<Licorice>(),
        ModelDb.Card<Lollipop>(),
        ModelDb.Card<MilkChocolate>(),
    ];
    
    public static CardModel CreateRandomTreatWithoutAddingToPile(Player owner, ICombatState combatState)
    {
        return combatState.CreateCard(owner.RunState.Rng.CombatCardGeneration.NextItem(CanonicalTreats)!, owner);
    }
    
    public static async Task<IEnumerable<CardModel>> CreateTreats(
        AjamaGhouliganCard sourceCard, PileType pile = PileType.Hand,
        CardPilePosition position = CardPilePosition.Bottom, int amountOverride = -1, Func<List<CardModel>, List<CardModel>>? modifyCardsBeforePreview = null)
    {
        int amount = amountOverride == -1 ? 
            sourceCard.DynamicVars.Treat.IntValue :
            amountOverride;
        
        return await CreateTreats(amount, sourceCard.Owner, sourceCard.CombatState!, pile, position, modifyCardsBeforePreview);
    }

    public static async Task<IEnumerable<CardModel>> CreateTreats(int amount,
        Player owner, ICombatState combatState, PileType pile = PileType.Hand,
        CardPilePosition position = CardPilePosition.Bottom, Func<List<CardModel>, List<CardModel>>? modifyCardsBeforePreview = null)
    {
        if (amount == 0 || CombatManager.Instance.IsOverOrEnding)
        {
            return [];
        }
        
        List<CardModel> cards = [];
        
        for (int i = 0; i < amount; i++)
        {
            cards.Add(combatState.CreateCard(owner.RunState.Rng.CombatCardGeneration.NextItem(CanonicalTreats)!, owner));
        }
        
        if (modifyCardsBeforePreview != null)
        {
            cards = modifyCardsBeforePreview(cards);
        }
        
        IReadOnlyList<CardPileAddResult> results = await CardPileCmd.AddGeneratedCardsToCombat(cards, pile, owner, position);

        if (pile != PileType.Hand) CardCmd.PreviewCardPileAdd(results);

        return cards;
    }

    public static async Task<IEnumerable<CardModel>> CreateSurprises(
        AjamaGhouliganCard sourceCard, PileType pile = PileType.Draw,
        CardPilePosition position = CardPilePosition.Random, int amountOverride = -1, bool preview = true, float previewTime = 1.2f)
    {
        int amount = amountOverride == -1 ? 
            sourceCard.DynamicVars.Surprise.IntValue :
            amountOverride;
        
        return await CreateSurprises(amount, sourceCard.Owner, sourceCard.CombatState!, pile, position, preview, previewTime);
    }

    public static async Task<IEnumerable<CardModel>> CreateSurprises(int amount,
        Player owner, ICombatState combatState, PileType pile = PileType.Draw,
        CardPilePosition position = CardPilePosition.Random, bool preview = true, float previewTime = 1.2f)
    {
        return await CreateCards(ModelDb.Card<Surprise>(), amount, owner, combatState, pile, position, preview, previewTime);
    }
    
    public static async Task<IEnumerable<CardModel>> CreateScorn(
        AjamaGhouliganCard sourceCard, PileType pile = PileType.Hand,
        CardPilePosition position = CardPilePosition.Bottom, int amountOverride = -1, bool preview = true, float previewTime = 1.2f)
    {
        int amount = amountOverride == -1 ? 
            sourceCard.DynamicVars.Scorn.IntValue :
            amountOverride;
        
        return await CreateScorn(amount, sourceCard.Owner, sourceCard.CombatState!, pile, position, preview, previewTime);
    }

    public static async Task<IEnumerable<CardModel>> CreateScorn(int amount,
        Player owner, ICombatState combatState, PileType pile = PileType.Hand,
        CardPilePosition position = CardPilePosition.Bottom, bool preview = true, float previewTime = 1.2f)
    {
        return await CreateCards(ModelDb.Card<Scorn>(), amount, owner, combatState, pile, position, preview, previewTime);
    }
}