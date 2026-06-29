using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class BuryVar : DynamicVar
{
    public const string Key = "Bury";
    
    public BuryVar(decimal baseValue, bool skipTooltip = false) : base(Key, baseValue)
    {
        if (!skipTooltip) this.WithTooltip("AJAMAGHOULIGAN-BURY_OTHER");
    }
}

public static class BuryVarExtension
{
    extension(DynamicVarSet dynamicVars)
    {
        public BuryVar Bury => (BuryVar) dynamicVars[BuryVar.Key];
    }
}