using CardGame.Data;

namespace CardGame;

public sealed class ShopManager : Singleton<ShopManager>
{
	[Property]
	public List<ShopItem> CardPacks { get; set; } = [];
	
	[Property]
	public List<ShopItem> Relics { get; set; } = [];
	
	[Property]
	public int RerollCost { get; set; } = ShopConfig.RerollCost;

	[Property]
	public int HealCost { get; set; } = 20;

	[Property]
	public int CardPackAmount { get; set; } = 6;

	[Property]
	public int RelicAmount { get; set; } = 4;

	[Property, InlineEditor]
	public List<Trade> Trades { get; set; } = [];
	
	public readonly Dictionary<CardPack.CardPackRarity, double> CardPackRarityChances = new()
	{
		{
			CardPack.CardPackRarity.Common, 0.6
		},
		{
			CardPack.CardPackRarity.Uncommon, 0.3
		},
		{
			CardPack.CardPackRarity.Rare, 0.15
		},
		{
			CardPack.CardPackRarity.Epic, 0.05
		}
	};

	public readonly Dictionary<CardPack.CardPackRarity, int> CardPackRarityCosts = new()
	{
		{
			CardPack.CardPackRarity.Common, 20
		},
		{
			CardPack.CardPackRarity.Uncommon, 30
		},
		{
			CardPack.CardPackRarity.Rare, 50
		},
		{
			CardPack.CardPackRarity.Epic, 100
		}
	};

	public readonly Dictionary<Relic.RelicRarity, double> RelicRarityChances = new()
	{
		{
			Relic.RelicRarity.Common, 0.6
		},
		{
			Relic.RelicRarity.Uncommon, 0.3
		},
		{
			Relic.RelicRarity.Rare, 0.15
		},
		{
			Relic.RelicRarity.Epic, 0.05
		}
	};

	public readonly Dictionary<Relic.RelicRarity, int> RelicRarityCosts = new()
	{
		{
			Relic.RelicRarity.Common, 50
		},
		{
			Relic.RelicRarity.Uncommon, 75
		},
		{
			Relic.RelicRarity.Rare, 100
		},
		{
			Relic.RelicRarity.Epic, 200
		}
	};
	
	public readonly Dictionary<Relic.RelicRarity, int> TradeInRequirements = new()
	{
		{
			Relic.RelicRarity.Common, 2
		},
		{
			Relic.RelicRarity.Uncommon, 2
		},
		{
			Relic.RelicRarity.Rare, 2
		}
	};
	
	public bool CanBuyItem( ShopItem item )
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return false;
		}

		return player.Money >= item.Cost;
	}
	
	public Id? FindTradeMatch( List<Id> relicsUsed )
	{
		foreach ( var trade in Trades )
		{
			if ( trade.Input.Count != relicsUsed.Count )
			{
				continue;
			}

			if ( !trade.Input.OrderBy( x => x ).SequenceEqual( relicsUsed.OrderBy( x => x ) ) )
			{
				continue;
			}

			return trade.Output;
		}

		return null;
	}
}

public static class ShopConfig
{
	public const int RerollCost = 5;
	public const int RerollCostIncrement = 5;

	public static class Weights
	{
		public const double KeywordMatchBonus = 2.0;
		public const double TypeMatchBonus = 1.5;
	}
}

public class ShopItem
{
	public int Cost { get; set; }
	public CardPack? Pack { get; set; }
	public Relic? Relic { get; set; }

	// ReSharper disable once ConvertToPrimaryConstructor
	public ShopItem( int cost, CardPack? pack )
	{
		Cost = cost;
		Pack = pack;
	}

	public ShopItem( int cost, Relic? relic )
	{
		Cost = cost;
		Relic = relic;
	}
}

public class Trade
{
	public List<Id> Input { get; set; } = [];
	
	public Id? Output { get; set; }
}
