using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using static AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core.BundledHoverTipManager;

namespace AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;

public class BundledHoverTip
{
    public string Name = "default";
    
    public Category SortCategory = Category.Middle;
    public int SortOrder = 0;
    
    public IEnumerable<IHoverTip> HoverTips = [];

    public int InternalSortOrder => SortOrder + (int) SortCategory;

    public BundledHoverTip() { }

    public BundledHoverTip(string name, IHoverTip hoverTip)
    {
        Name = name;
        HoverTips = [hoverTip];
    }
    
    public BundledHoverTip(string name, IEnumerable<IHoverTip> hoverTips)
    {
        Name = name;
        HoverTips = hoverTips;
    }
    
    public BundledHoverTip(string name, IHoverTip hoverTip, Category sortCategory)
    {
        Name = name;
        HoverTips = [hoverTip];
        SortCategory = sortCategory;
    }
    
    public BundledHoverTip(string name, IEnumerable<IHoverTip> hoverTips, Category sortCategory)
    {
        Name = name;
        HoverTips = hoverTips;
        SortCategory = sortCategory;
    }
}