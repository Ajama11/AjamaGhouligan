using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.HoverTips;

namespace AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips;

public class UnfortunateBundle() : BundledHoverTip(
    nameof(UnfortunateBundle),
    [
        HoverTipFactory.FromKeyword(MyEnums.Unfortunate),
        HoverTipFactory.FromPower<MisfortunePower>()
    ],
    BundledHoverTipManager.Category.End
)
{ }