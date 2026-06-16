using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace AjamaGhouligan.AjamaGhouliganCode.Scenes;

[GlobalClass]
public partial class GhouliganNEnergyCounter : NEnergyCounter
{
	private const string ScenePathIroncladFront = "res://scenes/vfx/energy/ironclad/ironclad_energy_vfx_front.tscn";
	private const string ScenePathIroncladBack = "res://scenes/vfx/energy/ironclad/ironclad_energy_vfx_back.tscn";

	private readonly Color _greenFireColor = new("32d172");

	public override void _Ready()
	{
		base._Ready();

		_frontVfx!._particles ??= [];
		_backVfx!._particles ??= [];

		var ironcladFront = ResourceLoader.Load<PackedScene>(ScenePathIroncladFront).Instantiate<NParticlesContainer>();
		var ironcladBack = ResourceLoader.Load<PackedScene>(ScenePathIroncladBack).Instantiate<NParticlesContainer>();

		foreach (var child in ironcladFront.GetChildren())
		{
			if (child is not GpuParticles2D particle) continue;

			particle.Reparent(_frontVfx);
			particle.GlobalPosition = _frontVfx.GlobalPosition;
			_frontVfx._particles.Add(particle);

			particle.SetSelfModulate(_greenFireColor);
		}

		foreach (var child in ironcladBack.GetChildren())
		{
			if (child is not GpuParticles2D particle) continue;

			particle.Reparent(_frontVfx);
			particle.GlobalPosition = _backVfx.GlobalPosition;
			_backVfx._particles.Add(particle);

			particle.SetSelfModulate(new Color("8effbc"));
		}

		ironcladFront.QueueFreeSafely();
		ironcladBack.QueueFreeSafely();
	}
}
