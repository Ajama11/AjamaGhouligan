using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class HalfSummonFilledVar(string name, decimal baseValue, bool skipTooltip = false) : SummonVar(name, baseValue)
{
    public const string Key = "HalfSummonFilled";
    public bool SkipTooltip = skipTooltip;
    
    public HalfSummonFilledVar(decimal baseValue, bool skipTooltip = false) : this(Key, baseValue, skipTooltip) { }
}

public static class HalfSummonFilledVarExtension
{
    extension(DynamicVarSet dynamicVars)
    {
        public HalfSummonFilledVar HalfSummonFilled => (HalfSummonFilledVar) dynamicVars[HalfSummonFilledVar.Key];
    }
}