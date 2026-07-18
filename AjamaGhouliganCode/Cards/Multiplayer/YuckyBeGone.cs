using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Multiplayer;

public class YuckyBeGone() : AjamaGhouliganCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    private const string TransformAmount = "TransformAmount";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<MisfortunePower>(6),
        ..MakeCalculatedVar(TransformAmount, 0,
            static (card, target) =>
                target == null ? 0 : GetStatusAndCurseCards(target.Player!).Count)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    public override IEnumerable<IHoverTip> MyHoverTips =>
    [
        HoverTipFactory.FromCard<Surprise>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        foreach (CardModel card in GetStatusAndCurseCards(play.Target!.Player!))
        {
            await CardCmd.TransformTo<Surprise>(card);
        }

        await MyActions.Misfortune(choiceContext, CombatState!.HittableEnemies, this);
    }

    private static List<CardModel> GetStatusAndCurseCards(Player player)
    {
        return player.PlayerCombatState!.AllCards
            .Where(c =>
                c.Pile != null &&
                c.Pile.Type != PileType.Exhaust &&
                c.Pile.Type != PileType.Play &&
                c.Type is CardType.Status or CardType.Curse)
            .ToList();
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<MisfortunePower>().UpgradeValueBy(3);
    }
}