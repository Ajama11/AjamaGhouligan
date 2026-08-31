using AjamaGhouligan.AjamaGhouliganCode.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.DynamicVars;

public class PattyCakeVar(string name, decimal baseValue, bool onPlayer = true, bool skipTooltip = false) : PowerVar<PattyCakePower>(name, baseValue)
{
    public const string Key = "PattyCakePower";
    public bool SkipTooltip = skipTooltip;
    public bool OnPlayer = onPlayer;

    public PattyCakeVar(decimal baseValue, bool onPlayer = true, bool skipTooltip = false) : this(Key, baseValue, onPlayer, skipTooltip) { }
}

public static class PattyCakeVarExtension
{
    extension(DynamicVarSet dynamicVars)
    {
        public PattyCakeVar PattyCake => (PattyCakeVar) dynamicVars[PattyCakeVar.Key];
    }
}