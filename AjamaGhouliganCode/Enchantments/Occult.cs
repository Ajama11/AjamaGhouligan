using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.HoverTips;

namespace AjamaGhouligan.AjamaGhouliganCode.Enchantments;

public class Occult : CustomEnchantmentModel
{
    protected override string CustomIconPath => "res://AjamaGhouligan/images/enchantments/occult.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(MyEnums.Haunted),
        HoverTipFactory.FromKeyword(MyEnums.Bury)
    ];

    protected override void OnEnchant()
    {
        Card.AddKeyword(MyEnums.Haunted);
        Card.AddKeyword(MyEnums.Bury);
    }
}