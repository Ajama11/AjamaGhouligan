using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Enchantments;

public class Occult : CustomEnchantmentModel
{
    protected override string CustomIconPath => "res://AjamaGhouligan/images/enchantments/occult.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(MyEnums.Grave)
    ];

    public override bool CanEnchant(CardModel card)
    {
        return base.CanEnchant(card) && !card.Keywords.Contains(MyEnums.Grave);
    }

    protected override void OnEnchant()
    {
        Card.AddKeyword(MyEnums.Grave);
    }
}