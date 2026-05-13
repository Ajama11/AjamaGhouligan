using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class LoseDoomVar : DynamicVar
{
    public const string Key = "LoseDoom";
    
    public LoseDoomVar(decimal baseValue) : base(Key, baseValue)
    {
        this.WithTooltip();
    }
}

public static class LoseDoomVarDynamicVarSetExtensions
{
    public static DynamicVar LoseDoom(this DynamicVarSet dynamicVars)
    {
        return dynamicVars[LoseDoomVar.Key];
    }
}