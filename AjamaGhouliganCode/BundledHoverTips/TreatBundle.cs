using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.HoverTips;

namespace AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips;

public class TreatBundle(bool upgraded = false) : BundledHoverTip(
    nameof(TreatBundle),
    MyEnums.TreatHovers(upgraded)
)
{ }
