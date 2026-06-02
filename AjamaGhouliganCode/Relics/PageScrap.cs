using AjamaGhouligan.AjamaGhouliganCode.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Runs;

namespace AjamaGhouligan.AjamaGhouliganCode.Relics;

public class PageScrap() : AjamaGhouliganRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    
    public override bool TryModifyCardRewardOptions(Player player, List<CardCreationResult> cardRewardOptions, CardCreationOptions creationOptions)
    {
        if (Owner != player || creationOptions.Source != CardCreationSource.Encounter) return false;

        CardModel? card = CardFactory.CreateForReward(Owner, 1,
                new CardCreationOptions([ModelDb.CardPool<NecrobinderCardPool>()], CardCreationSource.None,
                        creationOptions.RarityOdds)
                    .WithFlags(CardCreationFlags.NoModifyHooks | CardCreationFlags.NoCardPoolModifications))
            .FirstOrDefault()
            ?.Card;

        if (card == null) return false;

        CardCreationResult result = new CardCreationResult(card);
        result.ModifyCard(card, this);
        cardRewardOptions.Add(result);
        
        return true;
    }
}