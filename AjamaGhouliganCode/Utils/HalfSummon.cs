using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public static class HalfSummon
{
    public static IEnumerable<DynamicVar> MakeVars(string name, int filled, int empty, bool skipTooltip = false)
    {
        yield return new HalfSummonFilledVar(name + "Filled", filled, skipTooltip);
        yield return new HalfSummonEmptyVar(name + "Empty", empty, skipTooltip);
    }
    
    public static IEnumerable<DynamicVar> MakeVars(int filled, int empty, bool skipTooltip = false)
    {
        return MakeVars("HalfSummon", filled, empty, skipTooltip);
    }
    
    public static IHoverTip DynamicTip(DynamicVarSet vars)
    {
        const string str = "AJAMAGHOULIGAN-HALF_SUMMON_DYNAMIC";

        LocString title = HoverTipFactory.L10NStatic(str + ".title");
        LocString description = HoverTipFactory.L10NStatic(str + ".description");
        
        title.Add(vars.HalfSummonFilled);
        description.Add(vars.HalfSummonFilled);
        
        title.Add(vars.HalfSummonEmpty);
        description.Add(vars.HalfSummonEmpty);
        
        title.Add("HalfSummonTotal", vars.HalfSummonFilled.IntValue + vars.HalfSummonEmpty.IntValue);
        description.Add("HalfSummonTotal", vars.HalfSummonFilled.IntValue + vars.HalfSummonEmpty.IntValue);
        
        return new HoverTip(title, description);
    }
}