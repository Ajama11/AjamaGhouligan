using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Multiplayer;

public class HelpingHands() : AjamaGhouliganCard(3,
    CardType.Skill, CardRarity.Rare,
    TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    public override IEnumerable<IHoverTip> MyHoverTips =>
    [
        HoverTipFactory.FromKeyword(MyEnums.Haunted),
        HoverTipFactory.FromKeyword(MyEnums.Bury),
        HoverTipFactory.FromCard<HelpingHand>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        foreach (Creature creature in CombatState!
                     .GetTeammatesOf(Owner.Creature)
                     .Where(c =>
                         c is { IsAlive: true, IsPlayer: true } &&
                         c != Owner.Creature))
        {
            await MyActions.CreateCards(ModelDb.Card<HelpingHand>(), 1, creature.Player!, CombatState);
        }

        await PlayerChooses(Owner, choiceContext, SelectionScreenPrompt, this);
    }

    public static async Task PlayerChooses(Player player, PlayerChoiceContext choiceContext, LocString selectionScreenPrompt, AbstractModel sourceModel)
    {
        NGame.Instance!.CurrentRunNode!.GlobalUi.AddChildSafely(NSmokyVignetteVfx.Create(new Color("4dccbba9"), new Color(0.0f, 1.5f, 4f, 0.33f))!);
        NGame.Instance.CurrentRunNode.GlobalUi.AddChildSafely(NNightmareHandsVfx.Create()!);
        
        CardModel? card = (await CardSelectCmd.FromHand(choiceContext,
                player, 
                new CardSelectorPrefs(selectionScreenPrompt, 1), 
                _ => true, sourceModel))
            .FirstOrDefault();
            
        if (card == null) return;

        CardModel clone = card.CreateClone();

        MyActions.GainsHauntedAndBury(clone, false);
            
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(clone, SepulchrePile.PileType, player), 0.6F);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}