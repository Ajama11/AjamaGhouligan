using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class ScornVar(decimal baseValue, bool skipTooltip = false) : DynamicVar(Key, baseValue)
{
    public const string Key = "Scorn";
    public bool SkipTooltip = skipTooltip;
}

public static class ScornVarExtension
{
    extension(DynamicVarSet dynamicVars)
    {
        public ScornVar Scorn => (ScornVar) dynamicVars[ScornVar.Key];
    }
}