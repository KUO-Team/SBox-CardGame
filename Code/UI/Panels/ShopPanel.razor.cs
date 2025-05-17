using System;
using System.Linq;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI;

public partial class ShopPanel
{
	public List<ShopItem> Cards { get; set; } = [];
	public List<ShopItem> Relics { get; set; } = [];

	private object? LastPurchasedItem { get; set; }

	private readonly Dictionary<Card.CardRarity, double> _cardRarityChances = new()
	{
		{
			Card.CardRarity.Common, 0.6
		},
		{
			Card.CardRarity.Uncommon, 0.3
		},
		{
			Card.CardRarity.Rare, 0.15
		},
		{
			Card.CardRarity.Epic, 0.05
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

	private readonly Dictionary<Card.CardRarity, int> _cardRarityCosts = new()
	{
		{
			Card.CardRarity.Common, 10
		},
		{
			Card.CardRarity.Uncommon, 20
		},
		{
			Card.CardRarity.Rare, 50
		},
		{
			Card.CardRarity.Epic, 100
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

	public int CardAmount { get; set; } = 6;

	public int RelicAmount { get; set; } = 4;

	private static SaveManager? SaveManager => CardGame.SaveManager.Instance;
	private static RelicManager? RelicManager => CardGame.RelicManager.Instance;

	private Panel? _sellMenu;
	private Panel? _keywordSelection;
	private Panel? _typeSelection;

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
		Cards.Clear();
		Relics.Clear();

		var shopCards = CardDataList.All
			.Where( card => (card.Availabilities & Card.CardAvailabilities.Shop) != 0 )
			.ToList();

		AddRandomCards( shopCards, Cards, CardAmount );

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

	private void AddRandomCards( List<Card> cardList, List<ShopItem> outputList, int count )
	{
		if ( cardList.Count == 0 || !Player.Local.IsValid() )
		{
			return;
		}

		var weightedCards = GetWeightedCardList( cardList );

		for ( var i = 0; i < count && weightedCards.Count > 0; i++ )
		{
			var card = PickAndRemoveRandom( weightedCards );
			var cost = _cardRarityCosts.GetValueOrDefault( card.Rarity, 50 );

			if ( card.Type == Card.CardType.Item )
			{
				cost = Math.Max( 0, cost - 5 );
			}

			outputList.Add( new ShopItem
			{
				Card = card, Cost = cost
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

	private List<Card> GetWeightedCardList( List<Card> cardList )
	{
		return cardList
			.SelectMany( card =>
			{
				var weight = _cardRarityChances.GetValueOrDefault( card.Rarity, 0.0 );
				return Enumerable.Repeat( card, (int)(weight * 100) );
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

	public void HideSellMenu()
	{
		_sellMenu?.Hide();
	}

	public void SellCard( Card card )
	{
		if ( Player.Local is not {} player )
		{
			return;
		}

		if ( !player.Cards.Contains( card ) )
		{
			return;
		}

		var baseCost = _cardRarityCosts.GetValueOrDefault( card.Rarity, 10 );
		var sellValue = Math.Max( 1, baseCost / 2 );

		WarningPanel? warning = null;

		warning = WarningPanel.Create( "Sell Card", $"Sell {card.Name} for {sellValue}g?", [
				new Button("Yes", "", () =>
				{
					player.Money += sellValue;
					player.Cards.Remove( card );
					Log.Info( $"Sold card: {card.Name} for {sellValue} gold" );
					warning?.Delete();
				}),
				new Button("No", "", () =>
				{
					warning?.Delete();
				})
			]
		);
	}

	public void RerollByKeyword( string keyword )
	{
		_keywordSelection?.Hide();

		if ( !CanRerollByKeyword() )
			return;

		if ( Player.Local.IsValid() )
			Player.Local.Money -= RerollKeywordCost;

		RerollKeywordCost += 10;

		Cards.Clear();
		Relics.Clear();

		var shopCards = CardDataList.All
			.Where( card => (card.Availabilities & Card.CardAvailabilities.Shop) != 0 )
			.ToList();

		var matchingCards = shopCards
			.Where( card => card.Keywords.Contains( keyword ) )
			.ToList();

		var nonMatchingCards = shopCards
			.Except( matchingCards )
			.ToList();

		var keywordWeightedCards = GetKeywordWeightedCardList( matchingCards, nonMatchingCards );
		AddRandomCards( keywordWeightedCards, Cards, CardAmount );

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
		if ( !CanRerollByType() )
		{
			return;
		}

		if ( Player.Local.IsValid() )
		{
			Player.Local.Money -= RerollTypeCost;
		}

		RerollTypeCost += 10;

		Cards.Clear();
		Relics.Clear();

		var shopCards = CardDataList.All
			.Where( card => (card.Availabilities & Card.CardAvailabilities.Shop) != 0 )
			.ToList();

		var matchingCards = shopCards
			.Where( card => card.Type == type )
			.ToList();

		var nonMatchingCards = shopCards
			.Except( matchingCards )
			.ToList();

		var typeWeightedCards = GetTypeWeightedCardList( matchingCards, nonMatchingCards );
		AddRandomCards( typeWeightedCards, Cards, CardAmount );

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

	private List<Card> GetTypeWeightedCardList( List<Card> matching, List<Card> nonMatching )
	{
		var weightedMatching = matching.SelectMany( card =>
		{
			var weight = _cardRarityChances.GetValueOrDefault( card.Rarity, 0.0 );
			return Enumerable.Repeat( card, (int)(weight * 200) ); // higher weight
		} );

		var weightedNonMatching = nonMatching.SelectMany( card =>
		{
			var weight = _cardRarityChances.GetValueOrDefault( card.Rarity, 0.0 );
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

	public void BuyCard( ShopItem item )
	{
		if ( !CanBuyCard( item ) || Player.Local is not {} player || item.Card is null )
		{
			return;
		}

		PlayerData.Data.SeeCard( item.Card.Id );
		player.Cards.Add( item.Card );
		Cards.Remove( item );
		player.Money -= item.Cost;
		LastPurchasedItem = item.Card;

		Log.Info( $"Bought card: {item.Card.Name}" );
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

	private List<Card> GetKeywordWeightedCardList( List<Card> matching, List<Card> nonMatching )
	{
		var weightedMatching = matching.SelectMany( card =>
		{
			var weight = _cardRarityChances.GetValueOrDefault( card.Rarity, 0.0 );
			return Enumerable.Repeat( card, (int)(weight * 200) ); // double weight
		} );

		var weightedNonMatching = nonMatching.SelectMany( card =>
		{
			var weight = _cardRarityChances.GetValueOrDefault( card.Rarity, 0.0 );
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

	public bool CanBuyCard( ShopItem item )
	{
		return Player.Local?.Money >= item.Cost;
	}

	public bool CanBuyRelic( ShopItem item )
	{
		return Player.Local?.Money >= item.Cost;
	}

	public void OpenSellMenu()
	{
		_sellMenu?.Show();
	}

	public void Close()
	{
		this.Hide();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( _relicRarityCosts.Count, _cardRarityCosts.Count, RerollCost, Player.Local?.Money );
	}

	public class ShopItem
	{
		public int Cost { get; set; }
		public Card? Card { get; set; }
		public Relic? Relic { get; set; }
	}
}
