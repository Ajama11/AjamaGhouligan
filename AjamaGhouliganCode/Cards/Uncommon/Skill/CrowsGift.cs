using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Uncommon.Skill;

public class CrowsGift() : AjamaGhouliganCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DoomPower>(4)
    ];

    public override IEnumerable<IHoverTip> MyHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Retain),
        HoverTipFactory.FromKeyword(MyEnums.Haunted)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await MyActions.SelfDoom(choiceContext, this);
        
        List<CardModel> list = [];

        CardModel? attack = CardFactory.GetDistinctForCombat(Owner, 
            ModelDb.CardPool<ColorlessCardPool>()
                .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                .Where(c => c.Type == CardType.Attack), 
            1, Owner.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();
        
        CardModel? skill = CardFactory.GetDistinctForCombat(Owner, 
            ModelDb.CardPool<ColorlessCardPool>()
                .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                .Where(c => c.Type == CardType.Skill), 
            1, Owner.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();
        
        CardModel? power = CardFactory.GetDistinctForCombat(Owner, 
            ModelDb.CardPool<ColorlessCardPool>()
                .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                .Where(c => c.Type == CardType.Power), 
            1, Owner.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();

        if (attack != null) list = [..list, attack];
        if (skill != null) list = [..list, skill];
        if (power != null) list = [..list, power];
        
        if (IsUpgraded) CardCmd.Upgrade(list, CardPreviewStyle.HorizontalLayout);

        foreach (CardModel card in list)
        {
            card.AddKeyword(CardKeyword.Retain);
            MyActions.HauntSpecific(card, false);
        }

        CardModel? selectedCard = await CardSelectCmd.FromChooseACardScreen(choiceContext, list, Owner, true);

        if (selectedCard != null) await CardPileCmd.AddGeneratedCardToCombat(selectedCard, PileType.Hand, Owner);
    }
}