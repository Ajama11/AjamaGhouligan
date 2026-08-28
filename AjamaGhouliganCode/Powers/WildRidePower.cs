using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class WildRidePower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<MisfortunePower>()
    ];

    // Behavior handled in MisfortunePower and UnfortunateSingleton
}