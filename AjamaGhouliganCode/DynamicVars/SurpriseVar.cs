using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class SurpriseVar(decimal baseValue, bool skipTooltip = false) : DynamicVar(Key, baseValue)
{
    public const string Key = "Surprise";
    public bool SkipTooltip = skipTooltip;
}

public static class SurpriseVarDynamicVarSetExtensions
{
    public static DynamicVar Surprise(this DynamicVarSet dynamicVars)
    {
        return dynamicVars[SurpriseVar.Key];
    }
}