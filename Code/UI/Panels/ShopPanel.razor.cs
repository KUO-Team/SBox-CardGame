using System;
using Sandbox.UI;
using CardGame.Data;
using CardGame.UI.Map;

namespace CardGame.UI;

public partial class ShopPanel
{
	private object? LastPurchasedItem { get; set; }
	public List<Id> SelectedRelicsForTradeIn { get; set; } = [];

	public MapPanel? Map { get; set; }

	private Panel? _tradeMenu, _tradeInfo;
	private PackOpeningPanel? _packOpeningPanel;

	private const int KeywordWeightBonus = 50;
	private const int NonMatchingWeightMultiplier = 50;
	private const int RerollCostIncrement = 10;

	private static SaveManager? SaveManager => SaveManager.Instance;
	private static RelicManager? RelicManager => RelicManager.Instance;
	private static ShopManager? ShopManager => ShopManager.Instance;

	private static List<ShopItem> CardPacks => ShopManager?.CardPacks ?? [];
	private static List<ShopItem> Relics => ShopManager?.Relics ?? [];

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( firstTime )
		{
			SetRandomItems();
		}

		base.OnAfterTreeRender( firstTime );
	}

	public static void SetRandomItems()
	{
		CardPacks.Clear();
		Relics.Clear();
		PopulateItems();
	}

	private static void PopulateItems()
	{
		var availableCards = CardPackDataList.All
			.Where( card => (card.Availabilities & CardPack.CardPackAvailabilities.Shop) != 0 )
			.ToList();

		var ownedRelicIds = RelicManager?.Relics?.Select( r => r.Data.Id ).ToHashSet() ?? [];
		var availableRelics = RelicDataList.All
			.Where( relic => !ownedRelicIds.Contains( relic.Id ) &&
				(relic.Availabilities & Relic.RelicAvailabilities.Shop) != 0 )
			.ToList();

		AddRandomCards( availableCards, CardPacks, ShopManager?.CardPackAmount ?? 0 );
		AddRandomRelics( availableRelics, Relics, ShopManager?.RelicAmount ?? 0 );
	}

	private static void AddRandomCards( List<CardPack> cards, List<ShopItem> output, int count )
	{
		var player = Player.Local;
		if ( cards.Count == 0 || !player.IsValid() )
		{
			return;
		}

		var weighted = CreateWeightedList( cards, GetCardWeight );
		var used = new HashSet<Id>();

		while ( output.Count < count && weighted.Count > 0 )
		{
			var card = PickAndRemoveRandom( weighted );
			var id = card.Id;

			if ( !used.Add( id ) )
			{
				continue;
			}

			var cost = ShopManager?.CardPackRarityCosts?.GetValueOrDefault( card.Rarity, 50 ) ?? 50;
			output.Add( new ShopItem( cost, card ) );
		}
	}

	private static void AddRandomRelics( List<Relic> relics, List<ShopItem> output, int count )
	{
		var player = Player.Local;
		if ( relics.Count == 0 || !player.IsValid() )
		{
			return;
		}

		var weighted = CreateWeightedList( relics, GetRelicWeight );
		var used = new HashSet<Id>();

		while ( output.Count < count && weighted.Count > 0 )
		{
			var relic = PickAndRemoveRandom( weighted );
			var id = relic.Id;

			if ( !used.Add( id ) )
			{
				continue;
			}

			var cost = ShopManager?.RelicRarityCosts?.GetValueOrDefault( relic.Rarity, 50 ) ?? 50;
			output.Add( new ShopItem( cost, relic ) );
		}
	}

	private static double GetCardWeight( CardPack pack ) =>
		ShopManager?.CardPackRarityChances?.GetValueOrDefault( pack.Rarity, 0.0 ) ?? 0.0;

	private static double GetRelicWeight( Relic relic ) =>
		ShopManager?.RelicRarityChances?.GetValueOrDefault( relic.Rarity, 0.0 ) ?? 0.0;

	private static List<T> CreateWeightedList<T>( List<T> items, Func<T, double> getWeight ) =>
		items.SelectMany( item => Enumerable.Repeat( item, Math.Max( 1, (int)(getWeight( item ) * 100) ) ) )
			.OrderBy( _ => Game.Random.Next() ).ToList();

	private static T PickAndRemoveRandom<T>( List<T> list )
	{
		var index = Game.Random.Next( list.Count );
		var item = list[index];
		list.RemoveAt( index );
		return item;
	}

	public void Reroll()
	{
		if ( !ShopManager.IsValid() || !CanAfford( ShopManager.RerollCost ) ) return;

		DeductMoney( ShopManager.RerollCost );
		ShopManager.RerollCost += RerollCostIncrement;
		SetRandomItems();
	}

	private static bool HasKeyword<T>( T item, string keyword ) => item switch
	{
		CardPack pack => pack.Keywords?.Contains( keyword ) == true,
		Relic relic => relic.Keywords?.Contains( keyword ) == true,
		_ => false
	};

	public void BuyCardPack( ShopItem item )
	{
		if ( !CanPurchase( item ) )
		{
			return;
		}

		ShowConfirmation( $"Buy {item.Pack?.Name} for {item.Cost}g?", () => CompletePurchase( item ) );
	}

	public void BuyRelic( ShopItem item )
	{
		if ( !CanPurchase( item ) || item.Relic is null )
		{
			return;
		}

		ShowConfirmation( $"Buy {item.Relic.Name} for {item.Cost}g?", () => CompletePurchase( item ) );
	}

	private void CompletePurchase( ShopItem item )
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		DeductMoney( item.Cost );
		LastPurchasedItem = item.Pack ?? (object?)item.Relic;

		if ( item.Pack is not null )
		{
			player.CardPacks?.Add( item.Pack );
			SaveManager?.ActiveRunData?.CardPacks?.Add( item.Pack.Id );
			CardPacks.Remove( item );
			_packOpeningPanel?.Show();
		}
		else if ( item.Relic is not null )
		{
			AddRelic( item.Relic );
			Relics.Remove( item );
		}
	}

	private void ShowConfirmation( string message, System.Action onConfirm )
	{
		WarningPanel? warning = null;
		warning = WarningPanel.Create( "Purchase", message, [
			new Button( "Yes", "", () =>
			{
				onConfirm?.Invoke();
				warning?.Delete();
			} ),

			new Button( "No", "", () => warning?.Delete() )
		] );
	}

	public void ToggleRelicSelection( Id relicId )
	{
		if ( SelectedRelicsForTradeIn.Remove( relicId ) )
		{
			return;
		}

		var relic = RelicManager?.Relics?.FirstOrDefault( r => r.Data?.Id.Equals( relicId ) == true )?.Data;
		if ( relic is null || !CanAddToTrade( relic ) )
		{
			return;
		}

		SelectedRelicsForTradeIn.Add( relicId );
	}

	private bool CanAddToTrade( Relic relic )
	{
		if ( SelectedRelicsForTradeIn.Count == 0 )
		{
			return true;
		}

		var firstRelic = RelicManager?.Relics?.FirstOrDefault( r => r.Data.Id.Equals( SelectedRelicsForTradeIn[0] ) )?.Data;
	
		// Check if rarities match
		if ( firstRelic?.Rarity != relic.Rarity )
		{
			return false;
		}

		// Check if we've already reached the required amount for this rarity
		var requiredAmount = ShopManager?.TradeInRequirements?
			.GetValueOrDefault( relic.Rarity, int.MaxValue ) ?? int.MaxValue;
	
		return SelectedRelicsForTradeIn.Count < requiredAmount;
	}

	public void CompleteTradeIn()
	{
		if ( !CanCompleteTrade() )
		{
			return;
		}

		var outputRelic = GetTradeOutput();
		if ( outputRelic is null )
		{
			return;
		}

		RemoveSelectedRelics();
		AddRelic( outputRelic );
		SelectedRelicsForTradeIn.Clear();
		ToggleTradeMenu();
	}

	private Relic? GetTradeOutput()
	{
		var specificTradeOutput = TryGetSpecificTradeOutput();
		return specificTradeOutput ?? TryGetGenericTradeOutput();
	}

	private Relic? TryGetSpecificTradeOutput()
	{
		var tradeOutputId = ShopManager?.FindTradeMatch( SelectedRelicsForTradeIn );
		if ( tradeOutputId is null )
		{
			return null;
		}

		return RelicDataList.All.FirstOrDefault( r => r.Id.Equals( tradeOutputId ) );
	}

	private Relic? TryGetGenericTradeOutput()
	{
		var firstRelic = GetFirstSelectedRelic();
		if ( firstRelic is null )
		{
			return null;
		}

		var targetRarity = firstRelic.Rarity.GetNextRarity();
		var availableRelics = GetAvailableRelicsForRarity( targetRarity );

		return availableRelics.Count > 0
			? availableRelics[Game.Random.Next( availableRelics.Count )]
			: null;
	}

	private Relic? GetFirstSelectedRelic()
	{
		if ( SelectedRelicsForTradeIn.Count == 0 )
		{
			return null;
		}

		return RelicManager?.Relics?
			.FirstOrDefault( r => r.Data.Id.Equals( SelectedRelicsForTradeIn[0] ) )?.Data;
	}

	private List<Relic> GetAvailableRelicsForRarity( Relic.RelicRarity targetRarity )
	{
		var ownedRelicIds = RelicManager?.Relics?
			.Select( r => r.Data.Id )
			.ToHashSet() ?? [];

		return RelicDataList.All
			.Where( r => r.Rarity == targetRarity &&
				(r.Availabilities & Relic.RelicAvailabilities.Shop) != 0 &&
				!ownedRelicIds.Contains( r.Id ) )
			.ToList();
	}

	private void RemoveSelectedRelics()
	{
		foreach ( var id in SelectedRelicsForTradeIn )
		{
			var relic = RelicManager?.Relics?.FirstOrDefault( r => r.Data.Id.Equals( id ) );
			if ( relic is not null )
			{
				RelicManager?.RemoveRelic( relic );
			}
			RemoveRelicFromSave( id );
		}
	}

	public bool CanCompleteTrade()
	{
		if ( SelectedRelicsForTradeIn.Count == 0 )
		{
			return false;
		}

		// Can trade if there's a specific trade OR if generic trade is possible
		return CanDoSpecificTrade() || CanDoGenericTrade();
	}

	private bool CanDoSpecificTrade()
	{
		return ShopManager?.FindTradeMatch( SelectedRelicsForTradeIn ) is not null;
	}

	private bool CanDoGenericTrade()
	{
		var firstRelic = GetFirstSelectedRelic();
		if ( firstRelic is null )
		{
			return false;
		}

		var requiredAmount = ShopManager?.TradeInRequirements?
			.GetValueOrDefault( firstRelic.Rarity, 0 ) ?? 0;

		return SelectedRelicsForTradeIn.Count >= requiredAmount && AllSelectedRelicsHaveSameRarity();
	}

	private bool AllSelectedRelicsHaveSameRarity()
	{
		var firstRelic = GetFirstSelectedRelic();
		if ( firstRelic is null )
		{
			return false;
		}

		return SelectedRelicsForTradeIn.All( id =>
		{
			var relic = RelicManager?.Relics?
				.FirstOrDefault( r => r.Data.Id.Equals( id ) )?.Data;
			return relic?.Rarity == firstRelic.Rarity;
		} );
	}

	public static void Heal()
	{
		if ( !CanHeal() )
		{
			return;
		}

		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		player.Unit?.HealToMax();
		if ( ShopManager.IsValid() )
		{
			DeductMoney( ShopManager.HealCost );
		}
	}

	private static bool CanAfford( int cost )
	{
		return Player.Local?.Money >= cost;
	}

	private static bool CanPurchase( ShopItem item )
	{
		return ShopManager.IsValid() && ShopManager.CanBuyItem( item );
	}

	public static bool CanHeal()
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return false;
		}

		var unit = player.Unit;
		if ( unit is null )
		{
			return false;
		}

		return !unit.IsMaxHp && CanAfford( ShopManager?.HealCost ?? 0 );
	}

	private static void DeductMoney( int amount )
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		player.Money -= amount;
	}

	private void AddRelic( Relic relic )
	{
		AddRelicToSave( relic.Id );
		PlayerData.Data.SeeRelic( relic.Id );
		RelicManager?.AddRelic( relic );
		Map?.RelicGainPanel?.Show( relic );
	}

	private static void AddRelicToSave( Id id )
	{
		SaveManager?.ActiveRunData?.Relics?.Add( id );
		var run = PlayerData.Data.Runs?.FirstOrDefault( x => x.Index == SaveManager?.ActiveRunData?.Index );
		run?.Relics?.Add( id );
		PlayerData.Save();
	}

	private static void RemoveRelicFromSave( Id id )
	{
		SaveManager?.ActiveRunData?.Relics?.Remove( id );
		var run = PlayerData.Data.Runs?.FirstOrDefault( x => x.Index == SaveManager?.ActiveRunData?.Index );
		run?.Relics?.Remove( id );
		PlayerData.Save();
	}

	public void ToggleTradeMenu()
	{
		_tradeMenu?.ToggleClass( "hidden" );
	}

	public void ToggleTradeInfo()
	{
		_tradeInfo?.ToggleClass( "hidden" );
	}

	public void Close()
	{
		this.Hide();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( CardPacks.Count, Relics.Count, Player.Local?.Money );
	}
}
