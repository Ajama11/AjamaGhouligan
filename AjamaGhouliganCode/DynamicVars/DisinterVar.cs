using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class DisinterVar(decimal baseValue) : DynamicVar(Key, baseValue)
{
    public const string Key = "Disinter";
}

public static class DisinterVarDynamicVarSetExtensions
{
    public static DynamicVar Disinter(this DynamicVarSet dynamicVars)
    {
        return dynamicVars[DisinterVar.Key];
    }
}