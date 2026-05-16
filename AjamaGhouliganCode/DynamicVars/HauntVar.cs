using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class HauntVar : DynamicVar
{
    public const string Key = "Haunt";
    
    public HauntVar(decimal baseValue, bool skipTooltip = false) : base(Key, baseValue)
    {
        if (!skipTooltip) this.WithTooltip();
    }
}

public static class HauntVarDynamicVarSetExtensions
{
    public static DynamicVar Haunt(this DynamicVarSet dynamicVars)
    {
        return dynamicVars[HauntVar.Key];
    }
}