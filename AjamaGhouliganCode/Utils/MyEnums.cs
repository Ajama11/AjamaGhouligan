using AjamaGhouligan.AjamaGhouliganCode.Cards.Token.Treats;
using AjamaGhouligan.AjamaGhouliganCode.Character;
using BaseLib.Patches.Content;
using BaseLib.Patches.UI;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;

namespace AjamaGhouligan.AjamaGhouliganCode.Utils;

public class MyEnums
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Haunted;
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Entomb;
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Unfortunate;
    
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Grave;

    [CustomEnum]
    public static StaticHoverTip Haunt;
    
    [CustomEnum]
    public static StaticHoverTip Bury;
    
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
        
        const string str = "AJAMAGHOULIGAN-TREATS";

        LocString title = HoverTipFactory.L10NStatic(str + ".title");
        LocString description = HoverTipFactory.L10NStatic(str + ".description");
        
        description.Add("energyPrefix", CustomEnergyIconPatches.GetEnergyColorName(ModelDb.CardPool<GhouliganCardPool>().Id));
        
        return
        [
            new CycleHoverTip(treats),
            new HoverTip(title, description)
        ];
    }
}