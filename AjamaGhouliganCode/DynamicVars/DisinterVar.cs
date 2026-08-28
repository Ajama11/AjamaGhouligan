using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class DisinterVar : DynamicVar
{
    public const string Key = "Disinter";
    public bool SkipTooltip;
    
    public DisinterVar(decimal baseValue, bool skipTooltip = false) : base(Key, baseValue)
    {
        SkipTooltip = skipTooltip;
        if (!skipTooltip) this.WithTooltip();
    }
}

public static class DisinterVarExtension
{
    extension(DynamicVarSet dynamicVars)
    {
        public DisinterVar Disinter => (DisinterVar) dynamicVars[DisinterVar.Key];
    }
}