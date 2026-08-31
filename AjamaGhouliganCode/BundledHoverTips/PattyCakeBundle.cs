using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips;

public class PattyCakeBundle(int amount, bool onPlayer = true) : BundledHoverTip(
    nameof(PattyCakeBundle),
    MakeHoverTip(amount, onPlayer))
{
    public static HoverTip MakeHoverTip(int amount, bool onPlayer)
    {
        PattyCakePower power = ModelDb.Power<PattyCakePower>();

        LocString description = power.Description;
        
        description.Add("Amount", amount);
        description.Add("PassAmount", amount - 1);
        description.Add("OnPlayer", onPlayer);

        return new HoverTip(power, description.GetFormattedText(), false);
    }
}