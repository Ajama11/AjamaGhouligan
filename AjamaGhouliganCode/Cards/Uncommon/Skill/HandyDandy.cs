using AjamaGhouligan.AjamaGhouliganCode.CardPiles;
using AjamaGhouligan.AjamaGhouliganCode.Cards;
using AjamaGhouligan.AjamaGhouliganCode.DynamicVars;
using AjamaGhouligan.AjamaGhouliganCode.Powers;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaGhouligan.AjamaGhouliganCode.Cards.Uncommon.Skill;

public class HandyDandy() : AjamaGhouliganCard(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy), IAfterSepulchreAutoplayOnTurnStart
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new OstyDamageVar(12, DamageProps.card),
        new PowerVar<FreeManualAttackPower>(1)
    ];

    private bool IsSepulchreAutoPlay { get; set; }
    private bool ShouldDelayBuff { get; set; }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (!Osty.CheckMissingWithAnim(Owner))
        {
            await DamageCmd.Attack(DynamicVars.OstyDamage.BaseValue)
                .FromOsty(Owner.Osty!, this, play)
                .Targeting(play.Target!)
                .WithHitFx(VfxCmd.heavyBluntPath, tmpSfx: TmpSfx.heavyAttack)
                .WithHitVfxSpawnedAtBase()
                .Execute(choiceContext);
        }
        
        if (play.IsAutoPlay && IsSepulchreAutoPlay)
        {
            ShouldDelayBuff = true;
        }
        else
        {
            await CommonActions.ApplySelf<FreeManualAttackPower>(choiceContext, this);
        }
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card != this) return Task.CompletedTask;
        if (oldPileType != SepulchrePile.PileType) return Task.CompletedTask;
        if (card.Pile?.Type != PileType.Play) return Task.CompletedTask;

        IsSepulchreAutoPlay = true;
        
        return Task.CompletedTask;
    }

    public async Task AfterSepulchreAutoplayOnTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        var snapshottedShouldDelayBuff = ShouldDelayBuff;
        var snapshottedIsSepulchreAutoPlay = IsSepulchreAutoPlay;

        ShouldDelayBuff = false;
        IsSepulchreAutoPlay = false;
        
        if (snapshottedShouldDelayBuff && snapshottedIsSepulchreAutoPlay)
        {
            await CommonActions.ApplySelf<FreeManualAttackPower>(choiceContext, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.OstyDamage.UpgradeValueBy(6);
    }
}