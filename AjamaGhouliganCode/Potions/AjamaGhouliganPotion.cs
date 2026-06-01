using BaseLib.Abstracts;
using BaseLib.Utils;
using AjamaGhouligan.AjamaGhouliganCode.Character;
using AjamaGhouligan.AjamaGhouliganCode.Extensions;
using BaseLib.Extensions;

namespace AjamaGhouligan.AjamaGhouliganCode.Potions;

[Pool(typeof(GhouliganPotionPool))]
public abstract class AjamaGhouliganPotion : CustomPotionModel
{
    public override string CustomPackedImagePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();

    public override string CustomPackedOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".PotionImagePath();
}