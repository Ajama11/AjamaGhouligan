using BaseLib.Config;

namespace AjamaGhouligan.AjamaGhouliganCode;

internal class Config : SimpleModConfig
{
    public enum GoofAudio {Everyone, Self, Never}
    [ConfigHoverTip]
    public static GoofAudio PlayGoofAudio { get; set; } = GoofAudio.Everyone;
}