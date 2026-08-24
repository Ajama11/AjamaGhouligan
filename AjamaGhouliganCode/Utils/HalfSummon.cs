using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public static class HalfSummon
{
    public static IEnumerable<DynamicVar> MakeVars(string name, int filled, int total, bool skipTooltip = false)
    {
        yield return new HalfSummonFilledVar(name + "Filled", filled, skipTooltip);
        yield return new HalfSummonTotalVar(name + "Total", total, skipTooltip);
    }
    
    public static IEnumerable<DynamicVar> MakeVars(int filled, int total, bool skipTooltip = false)
    {
        return MakeVars("HalfSummon", filled, total, skipTooltip);
    }
    
    public static IHoverTip DynamicTip(DynamicVarSet vars)
    {
        return HoverTipFactory.Static(MyEnums.HalfSummonDynamic, vars.HalfSummonFilled, vars.HalfSummonTotal);
    }
}