using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class HauntVar : DynamicVar
{
    public const string Key = "Haunt";
    
    public HauntVar(decimal baseValue) : base(Key, baseValue)
    {
        this.WithTooltip();
    }
}

public static class HauntVarDynamicVarSetExtensions
{
    public static DynamicVar Haunt(this DynamicVarSet dynamicVars)
    {
        return dynamicVars[HauntVar.Key];
    }
}