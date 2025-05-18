using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI;

public partial class ShopPanel
{
	public List<ShopItem> CardPacks { get; set; } = [];
	public List<ShopItem> Relics { get; set; } = [];

	private object? LastPurchasedItem { get; set; }

	private readonly Dictionary<CardPack.CardPackRarity, double> _cardPackRarityChances = new()
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

	private readonly Dictionary<CardPack.CardPackRarity, int> _cardPackRarityCosts = new()
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

	private readonly Dictionary<Relic.RelicRarity, double> _relicRarityChances = new()
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

	private readonly Dictionary<Relic.RelicRarity, int> _relicRarityCosts = new()
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

	private readonly List<string> _keywords = ["Bleed", "Burn", "Discard", "Singleton", "Enchanted"];

	public int RerollCost { get; set; } = 5;

	public int RerollKeywordCost { get; set; } = 15;

	public int RerollTypeCost { get; set; } = 15;

	public int HealCost { get; set; } = 20;

	public int CardPackAmount { get; set; } = 6;

	public int RelicAmount { get; set; } = 4;

	private static SaveManager? SaveManager => CardGame.SaveManager.Instance;
	private static RelicManager? RelicManager => CardGame.RelicManager.Instance;

	private Panel? _tradeMenu;
	private Panel? _keywordSelection;
	private Panel? _typeSelection;

	private Relic.RelicRarity _targetTradeInRarity = Relic.RelicRarity.Uncommon;
	private readonly List<Id> _selectedRelicsForTradeIn = [];

	/// <summary>
	/// How many of each rarity needed for an upgrade?
	/// </summary>
	private readonly Dictionary<Relic.RelicRarity, int> _tradeInRequirements = new()
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

	/// <summary>
	/// Return the next rarity up from the current.
	/// </summary>
	private static Relic.RelicRarity GetNextRarityUp( Relic.RelicRarity current )
	{
		return current switch
		{
			Relic.RelicRarity.Common => Relic.RelicRarity.Uncommon,
			Relic.RelicRarity.Uncommon => Relic.RelicRarity.Rare,
			Relic.RelicRarity.Rare => Relic.RelicRarity.Epic,
			_ => Relic.RelicRarity.Epic
		};
	}

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( firstTime )
		{
			SetRandomItems();
		}

		base.OnAfterTreeRender( firstTime );
	}

	public void SetRandomItems()
	{
		CardPacks.Clear();
		Relics.Clear();

		var shopCards = CardPackDataList.All
			.Where( card => (card.Availabilities & CardPack.CardPackAvailabilities.Shop) != 0 )
			.ToList();

		AddRandomCardPacks( shopCards, CardPacks, CardPackAmount );

		var ownedRelicIds = new HashSet<Id>();

		if ( RelicManager?.Relics is not null )
		{
			ownedRelicIds = RelicManager.Relics
				.Select( r => r.Data!.Id )
				.ToHashSet();
		}

		var availableRelics = RelicDataList.All
			.Where( relic => !ownedRelicIds.Contains( relic.Id ) &&
				(relic.Availabilities & Relic.RelicAvailabilities.Shop) != 0 )
			.ToList();

		if ( availableRelics.Count > 0 )
		{
			AddRandomRelics( availableRelics, Relics, RelicAmount );
		}
	}

	private void AddRandomCardPacks( List<CardPack> cardList, List<ShopItem> outputList, int count )
	{
		if ( cardList.Count == 0 || !Player.Local.IsValid() )
		{
			return;
		}

		var weightedPacks = GetWeightedCardPackList( cardList );

		while ( outputList.Count < count && weightedPacks.Count > 0 )
		{
			var pack = PickAndRemoveRandom( weightedPacks );
			var cost = _cardPackRarityCosts.GetValueOrDefault( pack.Rarity, 50 );

			outputList.Add( new ShopItem
			{
				Pack = pack, Cost = cost
			} );
		}
	}

	private void AddRandomRelics( List<Relic> relicList, List<ShopItem> outputList, int count )
	{
		if ( relicList.Count == 0 || !Player.Local.IsValid() )
		{
			return;
		}

		var weightedRelics = GetWeightedRelicList( relicList );
		var usedIds = new HashSet<Id>();

		while ( outputList.Count < count && weightedRelics.Count > 0 )
		{
			var relic = PickAndRemoveRandom( weightedRelics );

			if ( !usedIds.Add( relic.Id ) )
			{
				continue;
			}

			var cost = _relicRarityCosts.GetValueOrDefault( relic.Rarity, 50 );

			outputList.Add( new ShopItem
			{
				Relic = relic, Cost = cost
			} );
		}
	}

	private List<CardPack> GetWeightedCardPackList( List<CardPack> cardPackList )
	{
		return cardPackList
			.SelectMany( pack =>
			{
				var weight = _cardPackRarityChances.GetValueOrDefault( pack.Rarity, 0.0 );
				return Enumerable.Repeat( pack, (int)(weight * 100) );
			} )
			.OrderBy( _ => Game.Random.Next() )
			.ToList();
	}

	private List<Relic> GetWeightedRelicList( List<Relic> relicList )
	{
		return relicList
			.SelectMany( relic =>
			{
				var weight = _relicRarityChances.GetValueOrDefault( relic.Rarity, 0.0 );
				return Enumerable.Repeat( relic, (int)(weight * 100) );
			} )
			.OrderBy( _ => Game.Random.Next() )
			.ToList();
	}

	private static T PickAndRemoveRandom<T>( List<T> list )
	{
		var index = Game.Random.Next( list.Count );
		var item = list[index];
		list.RemoveAt( index );
		return item;
	}

	public void Reroll()
	{
		if ( !CanReroll() )
		{
			return;
		}

		if ( Player.Local.IsValid() )
		{
			Player.Local.Money -= RerollCost;
		}

		RerollCost += 5;
		SetRandomItems();
	}

	public void OpenRerollKeywordMenu()
	{
		_keywordSelection?.Show();
	}

	public void OpenTypeSelection()
	{
		_typeSelection?.Show();
	}

	public void HideRerollKeywordMenu()
	{
		_keywordSelection?.Hide();
	}

	public void HideTypeSelection()
	{
		_typeSelection?.Hide();
	}

	public void RerollByKeyword( string keyword )
	{
		_keywordSelection?.Hide();

		if ( !CanRerollByKeyword() )
		{
			return;
		}

		if ( Player.Local.IsValid() )
		{
			Player.Local.Money -= RerollKeywordCost;
		}

		RerollKeywordCost += 10;

		CardPacks.Clear();
		Relics.Clear();

		var shopCards = CardPackDataList.All
			.Where( card => (card.Availabilities & CardPack.CardPackAvailabilities.Shop) != 0 )
			.ToList();

		var matchingCards = shopCards
			.Where( card => card.Keywords.Contains( keyword ) )
			.ToList();

		var nonMatchingCards = shopCards
			.Except( matchingCards )
			.ToList();

		var keywordWeightedCards = GetKeywordWeightedCardList( matchingCards, nonMatchingCards );
		AddRandomCardPacks( keywordWeightedCards, CardPacks, CardPackAmount );

		// Relics
		var ownedRelicIds = new HashSet<Id>();

		if ( RelicManager?.Relics is not null )
		{
			ownedRelicIds = RelicManager.Relics
				.Select( r => r.Data!.Id )
				.ToHashSet();
		}

		var shopRelics = RelicDataList.All
			.Where( relic => !ownedRelicIds.Contains( relic.Id ) &&
				(relic.Availabilities & Relic.RelicAvailabilities.Shop) != 0 )
			.ToList();

		var matchingRelics = shopRelics
			.Where( relic => relic.Keywords.Contains( keyword ) )
			.ToList();

		var nonMatchingRelics = shopRelics
			.Except( matchingRelics )
			.ToList();

		var keywordWeightedRelics = GetKeywordWeightedRelicList( matchingRelics, nonMatchingRelics );
		AddRandomRelics( keywordWeightedRelics, Relics, RelicAmount );
	}

	public void RerollByType( Card.CardType type )
	{
		_typeSelection?.Hide();

		if ( !CanRerollByType() )
		{
			return;
		}

		if ( Player.Local.IsValid() )
		{
			Player.Local.Money -= RerollTypeCost;
		}

		RerollTypeCost += 10;

		CardPacks.Clear();
		Relics.Clear();

		var shopCards = CardPackDataList.All
			.Where( card => (card.Availabilities & CardPack.CardPackAvailabilities.Shop) != 0 )
			.ToList();

		// Track how many cards of the specified type each pack contains
		var packTypeCountMap = new Dictionary<CardPack, int>();

		foreach ( var pack in shopCards )
		{
			var cardsOfType = 0;

			foreach ( var cardId in pack.Cards )
			{
				var card = CardDataList.GetById( cardId );

				if ( card is null )
				{
					continue;
				}

				if ( card.Type == type )
				{
					cardsOfType++;
				}
			}

			if ( cardsOfType > 0 )
			{
				packTypeCountMap[pack] = cardsOfType;
			}
		}

		// Separate packs with matching cards from those without
		var matchingCards = shopCards
			.Where( card => packTypeCountMap.ContainsKey( card ) )
			.ToList();

		var nonMatchingCards = shopCards
			.Except( matchingCards )
			.ToList();

		var typeWeightedCards = GetTypeWeightedCardList( matchingCards, nonMatchingCards, packTypeCountMap );
		AddRandomCardPacks( typeWeightedCards, CardPacks, CardPackAmount );

		// Relics stay the same as normal reroll
		var ownedRelicIds = new HashSet<Id>();

		if ( RelicManager?.Relics is not null )
		{
			ownedRelicIds = RelicManager.Relics
				.Select( r => r.Data!.Id )
				.ToHashSet();
		}

		var availableRelics = RelicDataList.All
			.Where( relic => !ownedRelicIds.Contains( relic.Id ) &&
				(relic.Availabilities & Relic.RelicAvailabilities.Shop) != 0 )
			.ToList();

		AddRandomRelics( availableRelics, Relics, RelicAmount );
	}

	private List<CardPack> GetTypeWeightedCardList( List<CardPack> matching, List<CardPack> nonMatching, Dictionary<CardPack, int> typeCountMap )
	{
		// Create weighted list based on how many cards of the specific type are in each pack
		var weightedMatching = matching.SelectMany( pack =>
		{
			var weight = _cardPackRarityChances.GetValueOrDefault( pack.Rarity, 0.0 );
			// Multiply by 100 base weight plus additional weight per matching card
			var countBonus = typeCountMap.GetValueOrDefault( pack, 0 ) * 50; // 50 more weight per matching card
			return Enumerable.Repeat( pack, (int)((weight * 100) + countBonus) );
		} );

		var weightedNonMatching = nonMatching.SelectMany( card =>
		{
			var weight = _cardPackRarityChances.GetValueOrDefault( card.Rarity, 0.0 );
			return Enumerable.Repeat( card, (int)(weight * 50) ); // lower weight
		} );

		return weightedMatching
			.Concat( weightedNonMatching )
			.OrderBy( _ => Game.Random.Next() )
			.ToList();
	}

	public void Heal()
	{
		if ( !CanHeal() )
		{
			return;
		}

		if ( Player.Local.IsValid() && Player.Local.Unit is not null )
		{
			Player.Local.Unit.Hp = Player.Local.Unit.MaxHp;
		}
	}

	public bool CanHeal()
	{
		if ( !Player.Local.IsValid() || Player.Local.Unit is null )
		{
			return false;
		}

		if ( Player.Local.Unit.Hp >= Player.Local.Unit.MaxHp )
		{
			return false;
		}

		return Player.Local.Money >= HealCost;
	}

	public bool CanReroll()
	{
		return Player.Local?.Money >= RerollCost;
	}

	public bool CanRerollByKeyword()
	{
		return Player.Local?.Money >= RerollKeywordCost;
	}

	public bool CanRerollByType()
	{
		return Player.Local?.Money >= RerollTypeCost;
	}

	public void BuyCardPack( ShopItem pack )
	{
		if ( !CanBuyCardPack( pack ) )
		{
			return;
		}
		
		if ( Player.Local.IsValid() )
		{
			Player.Local.Money -= pack.Cost;
		}
		
		LastPurchasedItem = pack;
	}

	public void BuyRelic( ShopItem item )
	{
		if ( !CanBuyRelic( item ) || Player.Local is not {} player || !SaveManager.IsValid() || !RelicManager.IsValid() || item.Relic is null )
		{
			return;
		}

		if ( SaveManager.ActiveRunData is not null )
		{
			SaveManager.ActiveRunData.Relics.Add( item.Relic.Id );
			var run = PlayerData.Data.Runs.FirstOrDefault( x => x.Index == SaveManager.ActiveRunData.Index );
			run?.Relics.Add( item.Relic.Id );
			PlayerData.Save();
		}

		PlayerData.Data.SeeRelic( item.Relic.Id );
		RelicManager.AddRelic( item.Relic );
		Relics.Remove( item );
		player.Money -= item.Cost;
		LastPurchasedItem = item.Relic;

		Log.Info( $"Bought relic: {item.Relic.Name}" );
	}

	private List<CardPack> GetKeywordWeightedCardList( List<CardPack> matching, List<CardPack> nonMatching )
	{
		var weightedMatching = matching.SelectMany( card =>
		{
			var weight = _cardPackRarityChances.GetValueOrDefault( card.Rarity, 0.0 );
			return Enumerable.Repeat( card, (int)(weight * 200) ); // double weight
		} );

		var weightedNonMatching = nonMatching.SelectMany( card =>
		{
			var weight = _cardPackRarityChances.GetValueOrDefault( card.Rarity, 0.0 );
			return Enumerable.Repeat( card, (int)(weight * 50) ); // lower weight
		} );

		return weightedMatching
			.Concat( weightedNonMatching )
			.OrderBy( _ => Game.Random.Next() )
			.ToList();
	}

	private List<Relic> GetKeywordWeightedRelicList( List<Relic> matching, List<Relic> nonMatching )
	{
		var weightedMatching = matching.SelectMany( relic =>
		{
			var weight = _relicRarityChances.GetValueOrDefault( relic.Rarity, 0.0 );
			return Enumerable.Repeat( relic, (int)(weight * 200) );
		} );

		var weightedNonMatching = nonMatching.SelectMany( relic =>
		{
			var weight = _relicRarityChances.GetValueOrDefault( relic.Rarity, 0.0 );
			return Enumerable.Repeat( relic, (int)(weight * 50) );
		} );

		return weightedMatching
			.Concat( weightedNonMatching )
			.OrderBy( _ => Game.Random.Next() )
			.ToList();
	}

	public bool CanBuyCardPack( ShopItem item )
	{
		return Player.Local?.Money >= item.Cost;
	}

	public bool CanBuyRelic( ShopItem item )
	{
		return Player.Local?.Money >= item.Cost;
	}

	public void OpenTradeMenu()
	{
		_tradeMenu?.Show();
	}

	public void HideTradeMenu()
	{
		_tradeMenu?.Hide();
	}

	public void ToggleRelicSelection( Id relicId )
	{
		if ( _selectedRelicsForTradeIn.Remove( relicId ) )
		{
			return;
		}

		// Get the rarity of the selected relic
		var relic = RelicManager?.Relics.FirstOrDefault( r => r.Data.Id.Equals( relicId ) );

		if ( relic?.Data.Rarity is null )
		{
			return;
		}
		{
			var rarity = relic.Data.Rarity;

			// Only add if it matches the current selection rarity
			if ( _selectedRelicsForTradeIn.Count == 0 )
			{
				// First selection, set the target trade-in rarity
				_targetTradeInRarity = GetNextRarityUp( rarity );
				_selectedRelicsForTradeIn.Add( relicId );
			}
			else
			{
				// Check if this relic has the same rarity as others selected
				var firstRelic = RelicManager?.Relics.FirstOrDefault( r => r.Data.Id.Equals( _selectedRelicsForTradeIn[0] ) );
				if ( firstRelic?.Data?.Rarity == rarity )
				{
					_selectedRelicsForTradeIn.Add( relicId );
				}
			}
		}
	}

	public bool CanCompleteTrade()
	{
		if ( _selectedRelicsForTradeIn.Count == 0 || RelicManager?.Relics is null )
		{
			return false;
		}

		// Get the first selected relic to determine its rarity
		var firstRelic = RelicManager.Relics.FirstOrDefault( r => r.Data.Id.Equals( _selectedRelicsForTradeIn[0] ) );
		if ( firstRelic?.Data?.Rarity is null )
		{
			return false;
		}

		var rarity = firstRelic.Data.Rarity;

		// Check if we have enough relics of the same rarity
		if ( !_tradeInRequirements.TryGetValue( rarity, out var requiredAmount ) )
		{
			return false;
		}

		return _selectedRelicsForTradeIn.Count >= requiredAmount;
	}

	public void CompleteTradeIn()
	{
		if ( !CanCompleteTrade() || !SaveManager.IsValid() || !RelicManager.IsValid() )
		{
			return;
		}

		// Get the first selected relic to determine its rarity
		var firstRelic = RelicManager.Relics.FirstOrDefault( r => r.Data.Id.Equals( _selectedRelicsForTradeIn[0] ) );
		if ( firstRelic?.Data?.Rarity is null )
		{
			return;
		}

		var currentRarity = firstRelic.Data.Rarity;
		var targetRarity = GetNextRarityUp( currentRarity );

		if ( !_tradeInRequirements.TryGetValue( currentRarity, out var requiredAmount ) )
		{
			return;
		}

		// Take only the required amount of relics
		var relicsToRemove = _selectedRelicsForTradeIn.Take( requiredAmount ).ToList();

		// Remove the traded-in relics
		foreach ( var relicId in relicsToRemove )
		{
			// Remove from player's collection
			var relic = RelicManager.Relics.FirstOrDefault( x => x.Data.Id.Equals( relicId ) );
			if ( relic is not null )
			{
				RelicManager.RemoveRelic( relic );
			}

			// Remove from save data
			if ( SaveManager.ActiveRunData is not null )
			{
				SaveManager.ActiveRunData.Relics.Remove( relicId );
				var run = PlayerData.Data.Runs.FirstOrDefault( x => x.Index == SaveManager.ActiveRunData.Index );
				run?.Relics.Remove( relicId );
				PlayerData.Save();
			}
		}

		// Get available relics of the target rarity that the player doesn't own
		var availableRelics = RelicDataList.All
			.Where( relic =>
				relic.Rarity == targetRarity &&
				(relic.Availabilities & Relic.RelicAvailabilities.Shop) != 0 && RelicManager.Relics.All( r => !r.Data.Id.Equals( relic.Id ) ) )
			.ToList();

		if ( availableRelics.Count > 0 )
		{
			var newRelic = availableRelics[Game.Random.Next( availableRelics.Count )];
			if ( SaveManager.ActiveRunData is not null )
			{
				SaveManager.ActiveRunData.Relics.Add( newRelic.Id );
				var run = PlayerData.Data.Runs.FirstOrDefault( x => x.Index == SaveManager.ActiveRunData.Index );
				run?.Relics.Add( newRelic.Id );
				PlayerData.Save();
			}

			PlayerData.Data.SeeRelic( newRelic.Id );
			RelicManager.AddRelic( newRelic );
			LastPurchasedItem = newRelic;

			Log.Info( $"Traded in {requiredAmount} {currentRarity} relics for: {newRelic.Name}" );
		}

		_selectedRelicsForTradeIn.Clear();
		HideTradeMenu();
	}

	public void Close()
	{
		this.Hide();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( _relicRarityCosts.Count, _cardPackRarityCosts.Count, RerollCost, Player.Local?.Money );
	}

	public class ShopItem
	{
		public int Cost { get; set; }
		public CardPack? Pack { get; set; }
		public Relic? Relic { get; set; }
	}
}
