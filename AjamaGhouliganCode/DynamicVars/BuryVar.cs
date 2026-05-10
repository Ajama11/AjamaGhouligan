using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class BuryVar(decimal baseValue) : DynamicVar(Key, baseValue)
{
    public const string Key = "Bury";
}

public static class BuryVarDynamicVarSetExtensions
{
    public static DynamicVar Bury(this DynamicVarSet dynamicVars)
    {
        return dynamicVars[BuryVar.Key];
    }
}