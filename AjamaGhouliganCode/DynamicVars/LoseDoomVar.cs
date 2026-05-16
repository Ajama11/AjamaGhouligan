using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class LoseDoomVar(decimal baseValue) : DynamicVar(Key, baseValue)
{
    public const string Key = "LoseDoom";
}

public static class LoseDoomVarDynamicVarSetExtensions
{
    public static DynamicVar LoseDoom(this DynamicVarSet dynamicVars)
    {
        return dynamicVars[LoseDoomVar.Key];
    }
}