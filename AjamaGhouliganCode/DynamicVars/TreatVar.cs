using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class TreatVar(decimal baseValue, bool skipTooltip = false) : DynamicVar(Key, baseValue)
{
    public const string Key = "Treat";
    public bool SkipTooltip = skipTooltip;
}

public static class TreatVarDynamicVarSetExtensions
{
    public static DynamicVar Treat(this DynamicVarSet dynamicVars)
    {
        return dynamicVars[TreatVar.Key];
    }
}