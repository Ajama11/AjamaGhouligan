using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Token;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using AjamaGhouligan.AjamaGhouliganCode.Character;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Extensions;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

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
            
            if (DynamicVars.ContainsKey(nameof(MisfortunePower)) || Keywords.Contains(MyEnums.Unfortunate))
            {
                result = [..result, HoverTipFactory.FromPower<MisfortunePower>()];
            }

            if (DynamicVars.ContainsKey(nameof(DoomPower)) || 
                DynamicVars.ContainsKey(nameof(DoomNextTurnPower)) ||
                (
                    DynamicVars.ContainsKey(LoseDoomVar.Key) && 
                    !((LoseDoomVar) DynamicVars.LoseDoom()).SkipTooltip
                ))
            {
                result = [..result, HoverTipFactory.FromPower<DoomPower>()];
            }
            
            if (DynamicVars.ContainsKey(nameof(GoofPower)))
            {
                result = [..result, HoverTipFactory.FromPower<GoofPower>()];
            }

            if (DynamicVars.ContainsKey(HauntVar.Key) && !((HauntVar) DynamicVars.Haunt()).SkipTooltip)
            {
                result = [..result, HoverTipFactory.Static(MyEnums.Haunt), HoverTipFactory.FromKeyword(MyEnums.Haunted)];
            }
            
            if (DynamicVars.ContainsKey(BuryVar.Key))
            {
                result = [..result, HoverTipFactory.Static(MyEnums.BuryOther)];
            }
            
            if (DynamicVars.ContainsKey(SurpriseVar.Key) && !((SurpriseVar) DynamicVars.Surprise()).SkipTooltip)
            {
                result = [..result, HoverTipFactory.FromCard<Surprise>()];
            }
            
            if (DynamicVars.ContainsKey(TreatVar.Key) && !((TreatVar) DynamicVars.Treat()).SkipTooltip)
            {
                result = [..result, ..MyEnums.TreatHovers()];
            }

            return result;
        }
    }

    protected override bool ShouldGlowRedInternal => CanonicalTags.Contains(CardTag.OstyAttack) && Owner.IsOstyMissing;
}