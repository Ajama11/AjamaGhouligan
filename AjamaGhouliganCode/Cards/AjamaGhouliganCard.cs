using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using AjamaGhouligan.AjamaGhouliganCode.Character;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Extensions;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards;

[Pool(typeof(GhouliganCardPool))]
public abstract class AjamaGhouliganCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    
    // public virtual IEnumerable<CardKeyword> MyCanonicalKeywords => [];
    public virtual HashSet<CardTag> MyCanonicalTags => [];
    public virtual IEnumerable<IHoverTip> MyHoverTips => [];
    
    // public override IEnumerable<CardKeyword> CanonicalKeywords
    // {
    //     get
    //     {
    //         IEnumerable<CardKeyword> result = [..MyCanonicalKeywords];
    //
    //         return result;
    //     }
    // }

    protected override HashSet<CardTag> CanonicalTags
    {
        get
        {
            HashSet<CardTag> result = [..MyCanonicalTags];

            if (DynamicVars.ContainsKey(OstyDamageVar.defaultName) || (DynamicVars.ContainsKey(CalculatedDamageVar.defaultName) && DynamicVars.CalculatedDamage.IsFromOsty))
            {
                result = [..result, CardTag.OstyAttack];
            }
            
            return result;
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            IEnumerable<IHoverTip> result = [..MyHoverTips];
            
            if (DynamicVars.ContainsKey(SummonVar.defaultName))
            {
                result = [..result, HoverTipFactory.Static(StaticHoverTip.SummonDynamic, DynamicVars.Summon)];
            }

            // if (DynamicVars.ContainsKey(HauntVar.Key))
            // {
            //     result = [..result, HoverTipFactory.FromKeyword(GhouliganEnums.Haunted)];
            // }

            return result;
        }
    }

    protected override bool ShouldGlowRedInternal => CanonicalTags.Contains(CardTag.OstyAttack) && Owner.IsOstyMissing;

    protected override PileType GetResultPileTypeForCardPlay()
    {
        if (!Keywords.Contains(MyEnums.Bury) || Type == CardType.Power) return base.GetResultPileTypeForCardPlay();

        PileType result = base.GetResultPileTypeForCardPlay();
        return result == PileType.Exhaust ? result : SepulchrePile.PileType;
    }
}