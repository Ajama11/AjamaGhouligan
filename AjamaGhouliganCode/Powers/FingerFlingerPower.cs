using AjamaGhouligan.AjamaGhouliganCode.Cards.Basic;
using AjamaGhouligan.AjamaGhouliganCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace AjamaGhouligan.AjamaGhouliganCode.Powers;

public class FingerFlingerPower : AjamaGhouliganPower, IOnDisinter
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(MyEnums.Disinter),
        HoverTipFactory.FromCard<Boop>()
    ];


    public async Task OnDisinter(CardModel card)
    {
        if (Owner.Player == null) return;
        
        Flash();

        for (int i = 0; i < Amount; i++)
        {
            CardModel boop = Owner.CombatState!.CreateCard<Boop>(Owner.Player);
            boop.IsDupe = true;

            await CardCmd.AutoPlay(new ThrowingPlayerChoiceContext(), boop, null);
        }
    }
}