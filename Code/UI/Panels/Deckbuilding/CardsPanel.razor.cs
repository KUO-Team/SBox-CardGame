using System;
using CardGame.Data;
using CardGame.Effects;
using CardGame.Units;
using Sandbox.UI;

namespace CardGame.UI;

public partial class CardsPanel
{
	private static Player? CurrentPlayer => Player.Local;

	private DropDown? _filter;
	private string _currentFilter = "All";
	private Panel? _slots;

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
		{
			return;
		}

		SetDefaultOption( _filter );
		base.OnAfterTreeRender( firstTime );
	}

	private static void SetDefaultOption( DropDown? dropdown )
	{
		if ( !dropdown.IsValid() )
		{
			return;
		}

		var t = dropdown.Children.FirstOrDefault();
		if ( !t.IsValid() )
		{
			return;
		}

		var t1 = t.Children.OfType<Label>().FirstOrDefault();
		if ( !t1.IsValid() )
		{
			return;
		}

		dropdown.Selected = new Option()
		{
			Title = t1.Text,
		};
	}

	private void OnFilterChanged()
	{
		if ( !_filter.IsValid() )
		{
			return;
		}

		_currentFilter = (string)_filter.Value;
		StateHasChanged();
	}

	private IEnumerable<IEnumerable<CardGroup>> GetGroupedCards()
	{
		if ( CurrentPlayer?.Cards is null )
			return [];

		var cardGroups = CurrentPlayer.Cards
			.GroupBy( c => c.Id );

		// Apply filtering based on selected option
		cardGroups = _currentFilter switch
		{
			"Rarity" => cardGroups
				.OrderByDescending( g => Convert.ToInt32( g.First().Rarity ) ),
			"Cost" => cardGroups
				.OrderByDescending( g => g.First().EffectiveCost.Mp ),
			"Type" => cardGroups
				.OrderByDescending( g => Convert.ToInt32( g.First().Type ) ),
			_ => cardGroups
				.OrderByDescending( g => Convert.ToInt32( g.First().Rarity ) )
		};

		var groupedCards = cardGroups
			.Select( g => new CardGroup
			{
				Card = g.First(), Count = g.Count()
			} )
			.ToList();

		return groupedCards
			.Select( ( card, index ) => new
			{
				Card = card, Index = index
			} )
			.GroupBy( x => x.Index / CardsPerRow )
			.Select( g => g.Select( x => x.Card ) );
	}

	private int CardsPerRow => 4;
	private float VisibleRows => 3f;

	private float CalculateScrollThumbHeight()
	{
		var totalRows = GetTotalRowCount();
		return Math.Min( VisibleRows / totalRows * 100f, 100f );
	}

	private float CalculateScrollThumbOffset()
	{
		if ( !_slots.IsValid() || !HasCards() )
			return 0f;

		var scrollPosition = _slots.ScrollOffset.y / _slots.ScrollSize.y;
		var scrollbarHeight = ScrollbarHeight;
		var thumbHeight = scrollbarHeight * CalculateScrollThumbHeight() / 100f;
		var availableScrollSpace = scrollbarHeight - thumbHeight;

		return Math.Max( 0, scrollPosition * availableScrollSpace );
	}

	private static float ScrollbarHeight => 1000f;

	private float GetTotalRowCount()
	{
		if ( !HasCards() )
		{
			return 1f;
		}

		var uniqueCardCount = CurrentPlayer!.Cards.GroupBy( c => c.Id ).Count();
		return (float)Math.Ceiling( uniqueCardCount / (float)CardsPerRow );
	}

	private bool HasCards()
	{
		return CurrentPlayer?.Cards is not null && CurrentPlayer.Cards.Count != 0;
	}

	public void AddCard( Card card )
	{
		if ( !CurrentPlayer.IsValid() )
		{
			return;
		}

		var unit = CurrentPlayer.Unit;
		if ( unit is null )
		{
			return;
		}

		switch ( card.Rarity )
		{
			// Max 1 Epic
			case Card.CardRarity.Epic:
				{
					var alreadyExists = unit.Deck.Any( id =>
					{
						var existing = CardDataList.GetById( id );
						return existing is not null && existing.Id.Equals( card.Id );
					} );

					if ( alreadyExists )
					{
						Log.Info( $"Cannot add duplicate epic card: {card.Id}" );
						return;
					}
					break;
				}
			// Max 3 Rare
			case Card.CardRarity.Rare:
				{
					var count = unit.Deck.Count( id =>
					{
						var existing = CardDataList.GetById( id );
						return existing is not null && existing.Id.Equals( card.Id );
					} );

					if ( count >= 3 )
					{
						Log.Info( $"Cannot add more than 3 copies of rare card: {card.Id}" );
						return;
					}
					break;
				}
			case Card.CardRarity.Common:
			case Card.CardRarity.Uncommon:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		var detail = new CardEffect.CardEffectDetail();
		foreach ( var action in card.Actions )
		{
			if ( action.Effect is null )
			{
				continue;
			}

			if ( !action.Effect.CanAddToDeck( detail ) )
			{
				return;
			}
		}

		if ( unit.Deck.Count >= 9 )
		{
			Log.Info( $"Deck is full ({9} cards). Cannot add more." );
			return;
		}

		unit.Deck.Add( card.Id );
		CurrentPlayer?.Cards.Remove( card );
	}

	public void Close()
	{
		WarningPanel? warning = null;

		if ( !CurrentPlayer.IsValid() )
		{
			CloseParent();
			return;
		}

		var unit = CurrentPlayer.Unit;
		if ( unit is null )
		{
			CloseParent();
			return;
		}

		switch ( unit.Deck.Count )
		{
			case 0:
				warning = WarningPanel.Create( "Incomplete Deck", "Your deck is empty! You must fill your deck to continue.", [
					new Button( "Okay", "", () =>
					{
						warning?.Delete();
					} )
				] );
				break;
			case < 9:
				warning = WarningPanel.Create( "Incomplete Deck", $"Your deck has only {unit.Deck.Count} cards. You need 9 cards to continue.", [
					new Button( "Okay", "", () =>
					{
						warning?.Delete();
					} )
				] );
				break;
			default:
				CloseParent();
				break;
		}
	}

	private void CloseParent()
	{
		Parent?.Hide();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( CurrentPlayer, CurrentPlayer?.Unit, CurrentPlayer?.Cards.Count, CurrentPlayer?.Unit?.Deck.Count, _slots?.ScrollOffset, _currentFilter );
	}

	private class CardGroup
	{
		public required Card Card { get; init; }
		public int Count { get; init; }
	}
}
