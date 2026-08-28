using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips;
using AjamaGhouligan.AjamaGhouliganCode.BundledHoverTips.Core;
using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards.Status;
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
using MegaCrit.Sts2.Core.Localization;
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

    public virtual HashSet<CardTag> MyCanonicalTags => [];

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

    public virtual BundledHoverTipManager MyBundles => [];
    public virtual List<List<string>> BundleReorders => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            BundledHoverTipManager bundles = [];

            #region Automatic Bundles
            
            if (Keywords.Contains(MyEnums.Haunted))
            {
                bundles.Add(new BundledHoverTip(
                    nameof(MyEnums.Haunted),
                    HoverTipFactory.FromKeyword(MyEnums.Haunted),
                    BundledHoverTipManager.Category.Start
                ));
            }
            
            foreach (var keyword in ((IEnumerable<CardKeyword>) CardKeywordOrder.beforeDescription).Reverse())
            {
                if (Keywords.Contains(keyword))
                {
                    bundles.Add(new BundledHoverTip(
                        keyword.GetTitle().GetRawText(),
                        HoverTipFactory.FromKeyword(keyword),
                        BundledHoverTipManager.Category.Start
                    ));

                    if (keyword == CardKeyword.Ethereal && !Keywords.Contains(CardKeyword.Exhaust))
                    {
                        bundles.Add(new BundledHoverTip(
                            CardKeyword.Exhaust.GetTitle().GetRawText(),
                            HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
                            BundledHoverTipManager.Category.Start
                        ));
                    }
                }
            }

            if (Keywords.Contains(MyEnums.Unfortunate))
            {
                bundles.Add(new UnfortunateBundle());
            }
            
            foreach (var keyword in CardKeywordOrder.afterDescription
                         .Where(k => k != MyEnums.Unfortunate))
            {
                if (Keywords.Contains(keyword))
                {
                    bundles.Add(new BundledHoverTip(
                        keyword.GetTitle().GetRawText(),
                        HoverTipFactory.FromKeyword(keyword),
                        BundledHoverTipManager.Category.End
                    ));
                }
            }

            if (GainsBlock)
            {
                bundles.Add(BundledHoverTipFactory.Static(StaticHoverTip.Block));
            }

            if (DynamicVars.ContainsKey(SummonVar.defaultName))
            {
                bundles.Add(new BundledHoverTip(
                    SummonVar.defaultName,
                    HoverTipFactory.Static(StaticHoverTip.SummonDynamic, DynamicVars.Summon)
                ));
            }
            
            if (DynamicVars.Values.Any(dv => dv is HalfSummonEmptyVar { SkipTooltip: false }))
            {
                bundles.Add(new BundledHoverTip(
                    nameof(HalfSummon),
                    HalfSummon.DynamicTip(DynamicVars)
                ));
            }
            
            if (DynamicVars.Values.Any(dv => dv is DisinterVar { SkipTooltip: false }))
            {
                bundles.Add(BundledHoverTipFactory.Static(MyEnums.Disinter));
            }
            
            if (DynamicVars.ContainsKey(nameof(MisfortunePower)))
            {
                bundles.Add(BundledHoverTipFactory.FromPower<MisfortunePower>());
            }

            if (DynamicVars.ContainsKey(nameof(DoomPower)) || 
                DynamicVars.ContainsKey(nameof(DoomNextTurnPower)))
            {
                bundles.Add(BundledHoverTipFactory.FromPower<DoomPower>());
            }
            
            if (DynamicVars.ContainsKey(nameof(GoofPower)))
            {
                bundles.Add(new GoofBundle());
            }

            if (DynamicVars.Values.Any(dv => dv is HauntVar { SkipTooltip: false }))
            {
                bundles.Add(new HauntBundle());
            }
            
            if (DynamicVars.Values.Any(dv => dv is BuryVar { SkipTooltip: false }))
            {
                bundles.Add(BundledHoverTipFactory.Static(MyEnums.BuryOther));
            }
            
            if (DynamicVars.Values.Any(dv => dv is SurpriseVar { SkipTooltip: false }))
            {
                bundles.Add(BundledHoverTipFactory.FromCard<Surprise>());
            }
            
            if (DynamicVars.Values.Any(dv => dv is LoseDoomVar { SkipTooltip: false }))
            {
                bundles.Add(BundledHoverTipFactory.FromPower<DoomPower>());
            }
            
            if (DynamicVars.Values.Any(dv => dv is TreatVar { SkipTooltip: false }))
            {
                bundles.Add(new TreatBundle(DynamicVars.Treat.Upgraded));
            }
            
            if (DynamicVars.Values.Any(dv => dv is ScornVar { SkipTooltip: false }))
            {
                bundles.Add(BundledHoverTipFactory.FromCard<Scorn>());
            }
            
            #endregion

            foreach (var bundle in MyBundles)
            {
                bundles.Add(bundle);
            }

            foreach (var list in BundleReorders)
            {
                bundles.Reorder(list[0], list.Skip(1).ToArray());
            }
            
            bundles.SortHoverTips();
            return bundles.GetHoverTips();
        }
    }

    protected override bool ShouldGlowRedInternal => CanonicalTags.Contains(CardTag.OstyAttack) && Owner.IsOstyMissing;
}