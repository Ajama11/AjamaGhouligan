using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class HalfSummonEmptyVar(string name, decimal baseValue, bool skipTooltip = false) : SummonVar(name, baseValue)
{
    public const string Key = "HalfSummonEmpty";
    public bool SkipTooltip = skipTooltip;
    
    public HalfSummonEmptyVar(decimal baseValue, bool skipTooltip = false) : this(Key, baseValue, skipTooltip) { }
}

public static class HalfSummonEmptyVarExtension
{
    extension(DynamicVarSet dynamicVars)
    {
        public HalfSummonEmptyVar HalfSummonEmpty => (HalfSummonEmptyVar) dynamicVars[HalfSummonEmptyVar.Key];
    }
}