using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class HauntVar : DynamicVar
{
    public const string Key = "Haunt";
    public bool SkipTooltip;
    
    public HauntVar(decimal baseValue, bool skipTooltip = false) : base(Key, baseValue)
    {
        SkipTooltip = skipTooltip;
        if (!skipTooltip) this.WithTooltip();
    }
}

public static class HauntVarExtension
{
    extension(DynamicVarSet dynamicVars)
    {
        public HauntVar Haunt => (HauntVar) dynamicVars[HauntVar.Key];
    }
}