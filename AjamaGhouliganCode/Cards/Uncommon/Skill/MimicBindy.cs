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
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Uncommon.Skill;

public class MimicBindy() : AjamaGhouliganCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DoomPower>(3)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [

    ];

    public override HashSet<CardTag> MyCanonicalTags =>
    [

    ];

    public override IEnumerable<IHoverTip> MyHoverTips =>
    [

    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await MyActions.SelfDoom(choiceContext, this);

        List<CardModel> list = CardFactory.GetDistinctForCombat(Owner,
            ModelDb.CardPool<NecrobinderCardPool>()
                .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                .Where(c => c.Type == CardType.Power), 3,
            Owner.RunState.Rng.CombatCardGeneration).ToList();

        CardModel? card = await CardSelectCmd.FromChooseACardScreen(choiceContext, list, Owner, true);
        if (card == null) return;
        
        card.SetToFreeThisTurn();
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<DoomPower>().UpgradeValueBy(-1);
    }
}