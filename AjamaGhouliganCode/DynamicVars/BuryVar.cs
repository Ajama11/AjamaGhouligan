using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class BuryVar : DynamicVar
{
    public const string Key = "Bury";
    
    public BuryVar(decimal baseValue, bool skipTooltip = false) : base(Key, baseValue)
    {
        if (!skipTooltip) this.WithTooltip();
    }
}

public static class BuryVarDynamicVarSetExtensions
{
    public static DynamicVar Bury(this DynamicVarSet dynamicVars)
    {
        return dynamicVars[BuryVar.Key];
    }
}