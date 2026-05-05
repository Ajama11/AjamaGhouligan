using Godot;
using System;
using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Character;
using BaseLib.Utils;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

public partial class NBuryPile : NCombatCardPile
{
    protected override PileType Pile => SepulchrePile.PileType;
    
    private const float HideOffsetX = 150f;
    private const float TooltipOffsetY = -300f;

    public static readonly Vector2 PilePosition = new Vector2(1826, 900);

    private static readonly string _scenePath = "res://AjamaGhouligan/scenes/bury_pile.tscn";
    private const string MegaLabelFont = "res://themes/kreon_bold_glyph_space_one.tres";

    public static AddedNode<NCombatPilesContainer, NBuryPile> _ = new(container =>
    {
        var buryPileButton = ResourceLoader.Load<PackedScene>(_scenePath).Instantiate<NBuryPile>();
        buryPileButton.Name = "%BuryPile";
        buryPileButton.Position = PilePosition;

        var background = buryPileButton.GetNode<TextureRect>("CountContainer/Background");
        background.Texture = ResourceLoader.Load<Texture2D>("res://images/packed/combat_ui/pile_button_count.png");

        var countLabel = buryPileButton.GetNode<GhouliganMegaLabel>("CountContainer/Count");
        var font = PreloadManager.Cache.GetAsset<Font>(MegaLabelFont);
        countLabel.AddThemeFontOverride(ThemeConstants.Label.Font, font);
        countLabel.MinFontSize = 20;
        countLabel.MaxFontSize = 26;

        return buryPileButton;
    });
    
    public override void _Ready()
    {
        ConnectSignals();
        _emptyPileMessage = new LocString("combat_messages", "OPEN_EMPTY_SEPULCHRE");

        _hoverTip = new HoverTip(new LocString("static_hover_tips", "SEPULCHRE_PILE.title"),
            new LocString("static_hover_tips", "SEPULCHRE_PILE.description"));

        Visible = false;
        SetAnimInOutPositions();
        Disable();
    }
    
    protected override void SetAnimInOutPositions()
    {
        _showPosition = Position;
        _hidePosition = Position + new Vector2(HideOffsetX, 0f);
    }

    public override void Initialize(Player player)
    {
        _localPlayer = player;
        _pile = Pile.GetPile(_localPlayer);
        _pile.ContentsChanged += HandleContentsChanged;
        _pile.CardAddFinished += AddCard;
        _pile.CardRemoveFinished += RemoveCard;

        _currentCount = _pile.Cards.Count;
        _countLabel.SetTextAutoSize(_currentCount.ToString());

        if (_pile.Cards.Count <= 0 && player.Character is not Ghouligan) return;
        Visible = true;
        Enable();
    }

    private void HandleContentsChanged()
    {
        _currentCount = _pile!.Cards.Count;
        // _countLabel.SetTextAutoSize(_currentCount.ToString());
        HandleVisibility();
    }
    
    protected override void OnFocus()
    {
        NHoverTipSet.Remove(this);
        var tooltip = NHoverTipSet.CreateAndShow(this, _hoverTip);
        tooltip!.GlobalPosition = GlobalPosition + new Vector2(-tooltip.TextHoverTipDimensions.X * 0.89f, TooltipOffsetY);
        _bumpTween?.Kill();
        _bumpTween = CreateTween();
        _bumpTween.TweenProperty(_icon, "scale", new Vector2(1.25f, 1.25f), 0.05);
    }

    protected override void AddCard()
    {
        base.AddCard();
        HandleVisibility();
    }

    public override void AnimIn()
    {
        base.AnimIn();
        Visible = true;
    }

    private void HandleVisibility()
    {
        if (_currentCount > 0)
        {
            AnimIn();
            Enable();
        }
        else if (_currentCount == 0 && _localPlayer!.Character is not Ghouligan)
        {
            AnimOut();
            Disable();
        }
    }
}
