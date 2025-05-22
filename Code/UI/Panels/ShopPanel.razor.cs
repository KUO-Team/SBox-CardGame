using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI;

public partial class ShopPanel
{
	private static List<ShopItem> CardPacks => ShopManager?.CardPacks ?? [];
	private static List<ShopItem> Relics => ShopManager?.Relics ?? [];

	private static int RerollCost => ShopManager?.RerollCost ?? 0;

	private static int RerollKeywordCost => ShopManager?.RerollKeywordCost ?? 0;

	private static int RerollTypeCost => ShopManager?.RerollTypeCost ?? 0;

	private static int HealCost => ShopManager?.HealCost ?? 0;

	private static int CardPackAmount => ShopManager?.CardPackAmount ?? 0;

	private static int RelicAmount => ShopManager?.RelicAmount ?? 0;

	private object? LastPurchasedItem { get; set; }

	private static SaveManager? SaveManager => SaveManager.Instance;
	private static RelicManager? RelicManager => RelicManager.Instance;
	private static ShopManager? ShopManager => ShopManager.Instance;

	private Panel? _tradeMenu;
	private Panel? _keywordSelection;
	private Panel? _typeSelection;
	private PackOpeningPanel? _packOpeningPanel;

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
			var cost = ShopManager?.CardPackRarityCosts.GetValueOrDefault( pack.Rarity, 50 ) ?? 0;

			outputList.Add( new ShopItem( cost, pack ) );
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

			var cost = ShopManager?.RelicRarityCosts.GetValueOrDefault( relic.Rarity, 50 ) ?? 0;
			outputList.Add( new ShopItem( cost, relic ) );
		}
	}

	private List<CardPack> GetWeightedCardPackList( List<CardPack> cardPackList )
	{
		return cardPackList
			.SelectMany( pack =>
			{
				var weight = ShopManager?.CardPackRarityChances.GetValueOrDefault( pack.Rarity, 0.0 ) ?? 0.0;
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
				var weight = ShopManager?.RelicRarityChances.GetValueOrDefault( relic.Rarity, 0.0 ) ?? 0.0;
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

		if ( ShopManager.IsValid() )
		{
			ShopManager.RerollCost += ShopManager.ShopConfig.RerollCostIncrement;
		}

		SetRandomItems();
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

		if ( ShopManager.IsValid() )
		{
			ShopManager.RerollKeywordCost += 10;
		}

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

		if ( ShopManager.IsValid() )
		{
			ShopManager.RerollTypeCost += 10;
		}

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
			var weight = ShopManager?.CardPackRarityChances.GetValueOrDefault( pack.Rarity, 0.0 ) ?? 0.0;
			// Multiply by 100 base weight plus additional weight per matching card
			var countBonus = typeCountMap.GetValueOrDefault( pack, 0 ) * 50; // 50 more weight per matching card
			return Enumerable.Repeat( pack, (int)((weight * 100) + countBonus) );
		} );

		var weightedNonMatching = nonMatching.SelectMany( card =>
		{
			var weight = ShopManager?.CardPackRarityChances.GetValueOrDefault( card.Rarity, 0.0 ) ?? 0.0;
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

		if ( Player.Local.IsValid() )
		{
			Player.Local.Unit?.HealToMax();
			Player.Local.Money -= HealCost;
		}
	}

	public void BuyCardPack( ShopItem item )
	{
		if ( !ShopManager.IsValid() )
		{
			return;
		}

		if ( !ShopManager.CanBuyItem( item ) )
		{
			return;
		}

		WarningPanel? warning = null;
		warning = WarningPanel.Create( "Buy Card Pack", $"Buy {item.Pack?.Name} for {item.Cost}g?", [
			new Button( "Yes", "", () =>
			{
				if ( Player.Local.IsValid() )
				{
					Player.Local.CardPacks.Add( item.Pack! );
					Player.Local.Money -= item.Cost;
				}
				SaveManager?.ActiveRunData?.CardPacks.Add( item.Pack!.Id );

				LastPurchasedItem = item;
				_packOpeningPanel?.Show();
				warning?.Delete();
			} ),
			new Button( "No", "", () =>
			{
				warning?.Delete();
			} )
		] );
	}

	public void BuyRelic( ShopItem item )
	{
		if ( !ShopManager.IsValid() )
		{
			return;
		}

		if ( !ShopManager.CanBuyItem( item ) || Player.Local is not {} player || !SaveManager.IsValid() || !RelicManager.IsValid() || item.Relic is null )
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

		WarningPanel? warning = null;
		warning = WarningPanel.Create( "Buy Relic", $"Buy {item.Relic?.Name} for {item.Cost}g?", [
			new Button( "Yes", "", () =>
			{
				PlayerData.Data.SeeRelic( item.Relic!.Id );
				RelicManager.AddRelic( item.Relic );
				Relics.Remove( item );
				player.Money -= item.Cost;
				LastPurchasedItem = item.Relic;
				warning?.Delete();
			} ),
			new Button( "No", "", () =>
			{
				warning?.Delete();
			} )
		] );
	}

	private List<CardPack> GetKeywordWeightedCardList( List<CardPack> matching, List<CardPack> nonMatching )
	{
		var weightedMatching = matching.SelectMany( card =>
		{
			var weight = ShopManager?.CardPackRarityChances.GetValueOrDefault( card.Rarity, 0.0 ) ?? 0.0;
			return Enumerable.Repeat( card, (int)(weight * 100 * ShopManager.ShopConfig.Weights.KeywordMatchBonus) );
		} );

		var weightedNonMatching = nonMatching.SelectMany( card =>
		{
			var weight = ShopManager?.CardPackRarityChances.GetValueOrDefault( card.Rarity, 0.0 ) ?? 0.0;
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
			var weight = ShopManager?.RelicRarityChances.GetValueOrDefault( relic.Rarity, 0.0 ) ?? 0.0;
			return Enumerable.Repeat( relic, (int)(weight * 100 * ShopManager.ShopConfig.Weights.KeywordMatchBonus) );
		} );

		var weightedNonMatching = nonMatching.SelectMany( relic =>
		{
			var weight = ShopManager?.RelicRarityChances.GetValueOrDefault( relic.Rarity, 0.0 ) ?? 0.0;
			return Enumerable.Repeat( relic, (int)(weight * 50) );
		} );

		return weightedMatching
			.Concat( weightedNonMatching )
			.OrderBy( _ => Game.Random.Next() )
			.ToList();
	}

	public void ToggleRelicSelection( Id relicId )
	{
		if ( _selectedRelicsForTradeIn.Remove( relicId ) )
		{
			return;
		}

		// Get the rarity of the selected relic
		var relic = RelicManager?.Relics.FirstOrDefault( r => r.Data.Id.Equals( relicId ) );

		if ( relic is null )
		{
			return;
		}
		{
			var rarity = relic.Data.Rarity;

			// Only add if it matches the current selection rarity
			if ( _selectedRelicsForTradeIn.Count == 0 )
			{
				// First selection, set the target trade-in rarity
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
	public void OpenTradeMenu()
	{
		_tradeMenu?.Show();
	}

	public void HideTradeMenu()
	{
		_tradeMenu?.Hide();
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

	public bool CanHeal()
	{
		if ( !Player.Local.IsValid() || Player.Local.Unit is null )
		{
			return false;
		}

		if ( Player.Local.Unit.IsMaxHp )
		{
			return false;
		}

		return Player.Local.Money >= HealCost;
	}

	public void Close()
	{
		this.Hide();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( ShopManager?.RelicRarityCosts.Count, ShopManager?.CardPackRarityCosts.Count, RerollCost, Player.Local?.Money );
	}
}
