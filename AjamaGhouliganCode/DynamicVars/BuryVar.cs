using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class BuryVar : DynamicVar
{
    public const string Key = "Bury";
    public bool SkipTooltip;
    
    public BuryVar(decimal baseValue, bool skipTooltip = false) : base(Key, baseValue)
    {
        SkipTooltip = skipTooltip;
    }
}

public static class BuryVarExtension
{
    extension(DynamicVarSet dynamicVars)
    {
        public BuryVar Bury => (BuryVar) dynamicVars[BuryVar.Key];
    }
}