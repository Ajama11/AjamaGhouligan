using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class TopsyTurvyPower : AjamaGhouliganPower, IBeforeSepulchreAutoplayOnTurnEnd
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(MyEnums.Bury)
    ];

    public async Task BeforeSepulchreAutoplayOnTurnEnd(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;

        await MyActions.SelectForBury(this, choiceContext, Owner.Player!, Amount, PileType.Hand, true);

        if (Owner.HasPower<EclipsePower>()) await Cmd.Wait(1f);
    }
}