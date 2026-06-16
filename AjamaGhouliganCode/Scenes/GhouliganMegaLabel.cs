using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;

namespace AjamaGhouligan.AjamaGhouliganCode.Scenes;

[GlobalClass]
public partial class GhouliganMegaLabel : MegaLabel
{
	private const string MegaLabelFont = "res://themes/kreon_bold_glyph_space_one.tres";

	public override void _Ready()
	{
		AddThemeFontOverride(ThemeConstants.Label.Font, PreloadManager.Cache.GetAsset<Font>(MegaLabelFont));
		base._Ready();
	}
}
