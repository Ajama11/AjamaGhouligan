using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Common.Skill;

public class MomentBefore() : AjamaGhouliganCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<MisfortunePower>(3),
        new CardsVar(1)
    ];

    public override BundledHoverTipManager MyBundles =>
    [
        BundledHoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        await MyActions.Misfortune(choiceContext, CombatState!.HittableEnemies, this);

        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars.Cards.IntValue);

        List<CardModel> cards = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs,
            c => !c.Keywords.Contains(CardKeyword.Retain),
            this))
            .ToList();

        foreach (CardModel card in cards)
        {
            CardCmd.ApplyKeyword(card, CardKeyword.Retain);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}