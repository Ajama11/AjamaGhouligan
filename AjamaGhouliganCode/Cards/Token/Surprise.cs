using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Audio;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Surprise() : AjamaGhouliganCard(0,
    CardType.Attack, CardRarity.Token,
    TargetType.AllEnemies)
{
    private const string CalculatedDraw = "CalculatedDraw";

    private static readonly ModSound[] Sounds = 
    [
        MySounds.BamLong,
        MySounds.BellHorn, MySounds.BellHorn,
        MySounds.Bonk, MySounds.Bonk,
        MySounds.Bronk, MySounds.Bronk,
        MySounds.Cymbal, MySounds.Cymbal,
        MySounds.CymbalTwo, MySounds.CymbalTwo,
        MySounds.Donk, MySounds.Donk,
        MySounds.Drum, MySounds.Drum,
        MySounds.FallCrash, MySounds.FallCrash,
        MySounds.Gun, MySounds.Gun,
        MySounds.GunTwo, MySounds.GunTwo,
        MySounds.KabongLong,
        MySounds.MetalPan, MySounds.MetalPan,
        MySounds.Tromboing, MySounds.Tromboing,
        MySounds.WhiskerPluck, MySounds.WhiskerPluck
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, DamageProps.card),
        ..MakeCalculatedVar(CalculatedDraw, 1, (card, _) => card.Owner.Creature.GetPowerAmount<KeepEmComingPower>())
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        foreach (Creature hittableEnemy in CombatState!.HittableEnemies)
        {
            NCombatRoom? instance = NCombatRoom.Instance;
            instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(hittableEnemy, VfxColor.Green));
        }
        
        SfxCmd.Play(FmodSfx.fire);
        Rng.Chaotic.NextItem(Sounds)!.Play(pitchVariation: 0.1f);
        
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .TargetingAllOpponents(CombatState!)
            .WithHitFx(VfxCmd.bluntPath, tmpSfx: TmpSfx.heavyAttack)
            .WithNoAttackerAnim()
            .Execute(choiceContext);

        await CardPileCmd.Draw(choiceContext, ((CalculatedVar) DynamicVars[CalculatedDraw]).Calculate(null), Owner);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card == this) await CardCmd.AutoPlay(choiceContext, this, null);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}