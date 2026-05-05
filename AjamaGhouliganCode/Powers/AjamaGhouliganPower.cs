using BaseLib.Abstracts;
using BaseLib.Extensions;
using AjamaGhouligan.AjamaGhouliganCode.Extensions;
using Godot;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public abstract class AjamaGhouliganPower : CustomPowerModel
{
    //Loads from AjamaGhouligan/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}