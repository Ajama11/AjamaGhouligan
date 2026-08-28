using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.HoverTips;

namespace AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips;

public class HauntBundle() : BundledHoverTip(
    nameof(HauntBundle),
    [
        HoverTipFactory.Static(MyEnums.Haunt),
        HoverTipFactory.FromKeyword(MyEnums.Haunted)
    ]
)
{ }
