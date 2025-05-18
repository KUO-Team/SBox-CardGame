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
	public int RerollKeywordCost { get; set; } = 15;

	[Property]
	public int RerollTypeCost { get; set; } = 15;

	[Property]
	public int HealCost { get; set; } = 20;

	[Property]
	public int CardPackAmount { get; set; } = 6;

	[Property]
	public int RelicAmount { get; set; } = 4;
	
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
			CardPack.CardPackRarity.Common, 10
		},
		{
			CardPack.CardPackRarity.Uncommon, 20
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
			Relic.RelicRarity.Common, 40
		},
		{
			Relic.RelicRarity.Uncommon, 60
		},
		{
			Relic.RelicRarity.Rare, 100
		},
		{
			Relic.RelicRarity.Epic, 200
		}
	};
	
	public readonly List<string> Keywords = ["Bleed", "Burn", "Discard", "Singleton", "Enchanted"];

	public bool CanBuyItem( ShopItem item )
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return false;
		}

		return player.Money >= item.Cost;
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
