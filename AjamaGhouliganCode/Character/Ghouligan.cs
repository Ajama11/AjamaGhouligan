using AjamaGhouligan.AjamaGhouliganCode.Cards.Basic;
using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using AjamaGhouligan.AjamaGhouliganCode.Extensions;
using AjamaGhouligan.AjamaGhouliganCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace AjamaGhouligan.AjamaGhouliganCode.Character;

public class Ghouligan : PlaceholderCharacterModel
{
    public const string CharacterId = "AjamaGhouligan";
    
    public override string PlaceholderID => "necrobinder";

    public static readonly Color Color = new("54cba2");

    public override Color MapDrawingColor => Color;

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 69;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Strike>(),
        // ModelDb.Card<Strike>(),
        // ModelDb.Card<Strike>(),
        // ModelDb.Card<Strike>(),
        ModelDb.Card<Defend>(),
        // ModelDb.Card<Defend>(),
        // ModelDb.Card<Defend>(),
        // ModelDb.Card<Defend>(),
        ModelDb.Card<Yoink>(),
        ModelDb.Card<Boop>(),
        ..CardPool.AllCards.Where(c => c.Rarity != CardRarity.Basic)
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<StolenPhylactery>(),
        ModelDb.Relic<PrismaticGem>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<GhouliganCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<GhouliganRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<GhouliganPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    
    public override NCreatureVisuals CreateCustomVisuals()
    {
        return NodeFactory<NCreatureVisuals>.CreateFromScene("res://AjamaGhouligan/scenes/ghouligan.tscn");
    }
    
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath => "character_icon_ghouligan.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_ghouligan.png".CharacterUiPath();
    
    public override string CustomTrailPath => SceneHelper.GetScenePath("vfx/card_trail_defect");

    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    public override float DeathAnimTime => 2f;
}