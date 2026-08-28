using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.HoverTips;

namespace AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips;

public class GoofBundle() : BundledHoverTip(
    nameof(GoofBundle),
    [
        HoverTipFactory.FromPower<GoofPower>(),
        HoverTipFactory.FromCard<Cavort>()
    ]
)
{ }
