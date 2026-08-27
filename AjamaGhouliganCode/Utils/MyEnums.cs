using AjamaGhouligan.AjamaGhouliganCode.Cards.Token.Treats;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public class MyEnums
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Haunted;
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Bury;
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Unfortunate;
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Grave;

    [CustomEnum]
    public static StaticHoverTip Haunt;
    
    [CustomEnum]
    public static StaticHoverTip BuryOther;
    
    [CustomEnum]
    public static StaticHoverTip Disinter;
    
    [CustomEnum]
    public static StaticHoverTip Treats;
    
    [CustomEnum]
    public static StaticHoverTip HalfSummonDynamic;

    [CustomEnum]
    public static CardTag Treat;

    public enum RandomBuryTargeting
    {
        All,
        NotHaunted,
        PrioritizeHaunted,
        OnlyHaunted
    }
    
    public static IEnumerable<IHoverTip> TreatHovers(bool upgraded = false)
    {
        List<CardModel> treats = MyActions.CanonicalTreats;

        if (upgraded)
        {
            treats = [];
            
            foreach (var canonicalTreat in MyActions.CanonicalTreats)
            {
                treats = [..treats, canonicalTreat.ToMutable()];
            }
            
            CardCmd.Upgrade(treats, CardPreviewStyle.None);
        }
        
        return
        [
            new CycleHoverTip(treats),
            HoverTipFactory.Static(Treats)
        ];
    }
}