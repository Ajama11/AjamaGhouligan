using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class DisinterVar : DynamicVar
{
    public const string Key = "Disinter";
    
    public DisinterVar(decimal baseValue, bool skipTooltip = false) : base(Key, baseValue)
    {
        if (!skipTooltip) this.WithTooltip();
    }
}

public static class DisinterVarDynamicVarSetExtensions
{
    public static DynamicVar Disinter(this DynamicVarSet dynamicVars)
    {
        return dynamicVars[DisinterVar.Key];
    }
}