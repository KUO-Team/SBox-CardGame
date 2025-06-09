using System;
using CardGame.Data;

namespace CardGame.UI;

public partial class DeckbuildingPanel
{
	private static Player? CurrentPlayer => Player.Local;

	public UnitData? UnitData { get; set; }
	
	private CardsPanel? _cardsPanel;
	
	private const int DesiredDeckSize = 9;
	
	public void Remove( Card card )
	{
		UnitData?.Deck.Remove( card.Id );
		CurrentPlayer?.Cards.Add( card );
	}

	public void RandomDeck()
	{
		if ( UnitData is null || !CurrentPlayer.IsValid() )
		{
			return;
		}

		var deck = UnitData.Deck;
		var collection = CurrentPlayer.Cards;

		// First, clear the deck
		foreach ( var cardId in deck.ToList() )
		{
			var card = CardDataList.GetById( cardId );
			if ( card is null )
			{
				continue;
			}
			
			deck.Remove( cardId );
			collection.Add( card );
		}

		var shuffled = collection.OrderBy( _ => Game.Random.Next() ).ToList();

		for ( int i = 0; i < Math.Min( DesiredDeckSize, shuffled.Count ); i++ )
		{
			var card = shuffled[i];
			collection.Remove( card );
			deck.Add( card.Id );
		}
		
		_cardsPanel?.StateHasChanged();
	}
	
	public void ClearDeck()
	{
		if ( UnitData is null )
		{
			return;
		}

		var deck = UnitData.Deck;
		foreach ( var cardId in UnitData.Deck.ToList() )
		{
			var card = CardDataList.GetById( cardId );
			if ( card is null )
			{
				continue;
			}
			
			deck.Remove( cardId );
			CurrentPlayer?.Cards.Add( card );
		}
		_cardsPanel?.StateHasChanged();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( CurrentPlayer?.Cards.Count, UnitData?.Deck.Count );
	}
}
