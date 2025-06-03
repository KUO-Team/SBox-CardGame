using System;
using CardGame.Data;

namespace CardGame.UI;

public partial class RewardsPanel
{
	[Parameter]
	public BattleRewards? Rewards { get; set; }

	public void ClaimReward( RewardType type )
	{
		if ( Rewards is null )
		{
			Log.Warning( $"Can't claim reward; no rewards set!" );
			return;
		}

		var player = Player.Local;
		if ( !player.IsValid() )
		{
			Log.Warning( $"Can't claim reward; no player!" );
			return;
		}

		switch ( type )
		{
			case RewardType.Card:
				{
					foreach ( var cardId in Rewards.Cards )
					{
						var card = CardDataList.GetById( cardId );
						if ( card is not null )
						{
							player.Cards.Add( card );
						}
					}
				}
				break;
			case RewardType.CardPack:
				{
					foreach ( var cardPackId in Rewards.CardPacks )
					{
						var cardPack = CardPackDataList.GetById( cardPackId );
						if ( cardPack is not null )
						{
							player.CardPacks.Add( cardPack );
						}
					}
				}
				break;
			case RewardType.Relic:
				{
					foreach ( var relicId in Rewards.Relics )
					{
						var relic = RelicDataList.GetById( relicId );
						if ( relic is not null )
						{
							RelicManager.Instance?.AddRelic( relic );
						}
					}
				}
				break;
			case RewardType.Money:
				player.Money += Rewards.Money;
				break;
			default:
				throw new ArgumentOutOfRangeException( nameof( type ), type, null );
		}
	}

	public enum RewardType
	{
		Card,
		CardPack,
		Relic,
		Money
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Rewards, Rewards?.Cards.Count, Rewards?.CardPacks.Count, Rewards?.Relics.Count, Rewards?.Money );
	}
}
