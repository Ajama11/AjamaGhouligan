using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class PranksterFormPower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<GoofPower>(),
        HoverTipFactory.FromPower<MisfortunePower>()
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner) return;

        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (cardPlay.Card.Type == CardType.Attack)
            await PowerCmd.Apply<GoofPower>(choiceContext, Owner, Amount, Owner, null);
        
        if (cardPlay.Card.Type == CardType.Skill)
            await PowerCmd.Apply<MisfortunePower>(choiceContext, CombatState.HittableEnemies, Amount, Owner, null);
    }
}