using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class HalfSummonTotalVar(string name, decimal baseValue, bool skipTooltip = false) : SummonVar(name, baseValue)
{
    public const string Key = "HalfSummonTotal";
    public bool SkipTooltip = skipTooltip;
    
    public HalfSummonTotalVar(decimal baseValue, bool skipTooltip = false) : this(Key, baseValue, skipTooltip) { }
}

public static class HalfSummonTotalVarExtension
{
    extension(DynamicVarSet dynamicVars)
    {
        public HalfSummonTotalVar HalfSummonTotal => (HalfSummonTotalVar) dynamicVars[HalfSummonTotalVar.Key];
    }
}