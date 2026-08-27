using MegaCrit.Sts2.Core.Entities.Powers;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class WildRidePower : AjamaGhouliganPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    
    // Behavior handled in MisfortunePower and UnfortunateSingleton
}