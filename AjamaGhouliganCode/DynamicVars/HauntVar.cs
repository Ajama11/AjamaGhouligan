using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class HauntVar(decimal baseValue) : DynamicVar(Key, baseValue)
{
    public const string Key = "Haunt";
}

public static class HauntVarDynamicVarSetExtensions
{
    public static DynamicVar Haunt(this DynamicVarSet dynamicVars)
    {
        return dynamicVars[HauntVar.Key];
    }
}