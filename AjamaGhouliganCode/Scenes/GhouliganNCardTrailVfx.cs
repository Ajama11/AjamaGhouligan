using Godot;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace AjamaGhouligan.AjamaGhouliganCode.Scenes;

[GlobalClass]
public partial class GhouliganNCardTrailVfx : NCardTrailVfx
{
	public override void _Ready()
	{
		const string additiveMaterial = "res://themes/canvas_item_material_additive_shared.tres";

		var outerTrail = GetNode<GhouliganNCardTrail>("%OuterTrail");
		outerTrail.Texture = ResourceLoader.Load<CompressedTexture2D>("res://images/packed/vfx/trail.png");
		outerTrail.SetMaterial(ResourceLoader.Load<Material>(additiveMaterial));

		var innerTrail = GetNode<GhouliganNCardTrail>("%InnerTrail");
		innerTrail.Texture = ResourceLoader.Load<CompressedTexture2D>("res://images/packed/vfx/trail2.png");
		innerTrail.SetMaterial(ResourceLoader.Load<Material>(additiveMaterial));

		var bigSparks = GetNode<CpuParticles2D>("%BigSparks");
		bigSparks.Texture = ResourceLoader.Load<CompressedTexture2D>("res://images/vfx/brush_particle_2.png");
		bigSparks.SetMaterial(ResourceLoader.Load<Material>(additiveMaterial));

		var littleSparks = GetNode<CpuParticles2D>("%LittleSparks");
		littleSparks.Texture = ResourceLoader.Load<CompressedTexture2D>("res://images/vfx/vfx_ghostly_power_up/sparkle.png");
		littleSparks.SetMaterial(ResourceLoader.Load<Material>(additiveMaterial));

		var outerSmallCard = GetNode<Sprite2D>("%Sprite2D2");
		outerSmallCard.Texture = ResourceLoader.Load<CompressedTexture2D>("res://images/packed/vfx/small_card_silhouette.png");
		outerSmallCard.SetMaterial(ResourceLoader.Load<Material>(additiveMaterial));

		var innerSmallCard = GetNode<Sprite2D>("%Sprite2D3");
		innerSmallCard.Texture = ResourceLoader.Load<CompressedTexture2D>("res://images/packed/vfx/small_card_silhouette.png");
		innerSmallCard.SetMaterial(ResourceLoader.Load<Material>(additiveMaterial));

		base._Ready();
	}
}
