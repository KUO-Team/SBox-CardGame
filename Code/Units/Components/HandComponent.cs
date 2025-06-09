using CardGame.Data;
using CardGame.Effects;
using CardGame.UI;

namespace CardGame.Units;

public class HandComponent : Component, IOwnable
{
	[Property, RequireComponent]
	public BattleUnitComponent? Owner { get; set; }

	[Property, WideMode]
	public List<Card> Hand { get; set; } = [];

	[Property, WideMode]
	public List<Card> Deck { get; set; } = [];

	[Property]
	public int MaxCards { get; set; } = 9;

	[Property]
	public bool IsFull => Deck.Count >= MaxCards;

	[Property]
	public bool IsEmpty => Deck.Count == 0;

	[Property, ReadOnly]
	public bool IsDiscardMode { get; private set; }

	public HandPanel? Panel { get; set; }

	public Card? DiscardModeActivator { get; set; }

	private int _maxDiscardableCards = 3;

	protected override void OnStart()
	{
		HandPanel.SelectedCards?.Clear();
		
		if ( Deck.Count > 0 )
		{
			foreach ( var card in Hand.ToArray() )
			{
				var newCard = card.DeepCopy();
				Hand.Remove( card );
				Hand.Add( newCard );
			}

			foreach ( var card in Deck.ToArray() )
			{
				var newCard = card.DeepCopy();
				Deck.Remove( card );
				Deck.Add( newCard );
			}
		}

		if ( BattleManager.Instance.IsValid() )
		{
			BattleManager.Instance.OnCombatStart += LeaveDiscardMode;
		}

		base.OnStart();
	}
	
	protected override void OnDisabled()
	{
		HandPanel.SelectedCards?.Clear();
		base.OnDisabled();
	}

	public void EnterDiscardMode( Card activator, int maxCards = 3 )
	{
		if ( !BattleManager.Instance.IsValid() )
		{
			return;
		}

		IsDiscardMode = true;
		DiscardModeActivator = activator;
		_maxDiscardableCards = maxCards;

		HandPanel.OnCardSelected += OnCardSelected;
		HandPanel.OnCardDeselected += OnCardDeselected;
	}

	private void OnCardSelected( Card card )
	{
		var selectedCards = HandPanel.SelectedCards;
    
		if ( selectedCards.Contains( card ) )
		{
			selectedCards.Remove( card );
			return;
		}

		if ( selectedCards.Count >= _maxDiscardableCards )
		{
			return;
		}

		selectedCards.Add( card );
	}

	private static void OnCardDeselected( Card card )
	{
		var selectedCards = HandPanel.SelectedCards;
		if ( !selectedCards.Contains( card ) )
		{
			return;
		}

		selectedCards.Remove( card );
	}

	public void ConfirmDiscard()
	{
		if ( !Owner.IsValid() )
		{
			return;
		}

		if ( DiscardModeActivator is null )
		{
			return;
		}

		var selectedCards = HandPanel.SelectedCards;
		foreach ( var card in selectedCards )
		{
			foreach ( var action in DiscardModeActivator.Actions )
			{
				if ( action.Type != Action.ActionType.Effect )
				{
					continue;
				}

				var detail = new CardEffect.CardEffectDetail
				{
					Unit = Owner
				};

				action.Effect?.OnDiscardModeCardDiscard( detail, card );
			}

			foreach ( var action in card.Actions )
			{
				var detail = new CardEffect.CardEffectDetail
				{
					Unit = Owner
				};

				action.Effect?.OnDiscard( detail );
			}

			foreach ( var status in Owner?.StatusEffects ?? [] )
			{
				status.OnDiscardCard( card );
			}

			foreach ( var passive in Owner?.Passives ?? [] )
			{
				passive.OnDiscardCard( card );
			}

			foreach ( var relic in RelicManager.Instance?.Relics ?? [] )
			{
				if ( Owner.IsValid() )
				{
					relic.OnDiscardCard( card, Owner );
				}
			}

			Hand.Remove( card );
			if ( card.Type != Card.CardType.Item )
			{
				Deck.Add( card );
			}
		}

		LeaveDiscardMode();
	}

	private void LeaveDiscardMode()
	{
		IsDiscardMode = false;
		DiscardModeActivator = null;
		HandPanel.OnCardSelected -= OnCardSelected;
		HandPanel.OnCardDeselected -= OnCardDeselected;
	}

	public void Draw()
	{
		var deck = Deck.ToArray();
		var card = Game.Random.FromArray( deck );

		if ( card is null )
		{
			return;
		}

		Deck.Remove( card );
		Hand.Add( card );

		foreach ( var action in card.Actions )
		{
			if ( action.Effect is not {} effect )
			{
				continue;
			}

			var detail = new CardEffect.CardEffectDetail
			{
				Unit = Owner
			};

			effect.OnDraw( detail );
		}
	}

	public void Draw( Id id )
	{
		var card = CardDataList.GetById( id );
		if ( card is null )
		{
			return;
		}

		var copy = card.DeepCopy();
		Hand.Add( copy );
	}

	public void Draw( Card card, bool fromDeck = false )
	{
		Hand.Add( card );

		if ( fromDeck )
		{
			Deck.Remove( card );
		}
	}

	public void DrawX( int x )
	{
		for ( var i = 0; i < x; i++ )
		{
			Draw();
		}
	}

	public void DrawX( Id id, int x )
	{
		for ( var i = 0; i < x; i++ )
		{
			var card = CardDataList.GetById( id );
			if ( card is null )
			{
				continue;
			}

			var copy = card.DeepCopy();
			Hand.Add( copy );
		}
	}

	public void Discard( Card card )
	{
		if ( !Owner.IsValid() )
		{
			return;
		}

		if ( !Hand.Contains( card ) )
		{
			return;
		}

		Hand.Remove( card );
		Deck.Add( card );

		foreach ( var action in card.Actions )
		{
			if ( action.Effect is not {} effect )
			{
				continue;
			}

			var detail = new CardEffect.CardEffectDetail
			{
				Unit = Owner
			};

			effect.OnDiscard( detail );

			if ( RelicManager.Instance.IsValid() )
			{
				foreach ( var relic in RelicManager.Instance.Relics )
				{
					relic.OnDiscardCard( card, Owner! );
				}
			}

			if ( Owner.StatusEffects.IsValid() )
			{
				foreach ( var statusEffect in Owner.StatusEffects )
				{
					statusEffect.OnDiscardCard( card );
				}
			}

			if ( Owner.Passives.IsValid() )
			{
				foreach ( var passive in Owner.Passives )
				{
					passive.OnDiscardCard( card );
				}
			}
		}
	}

	public void DiscardHand()
	{
		foreach ( var card in Hand.ToArray() )
		{
			Discard( card );
		}
	}

	public void Exhaust( Card card )
	{
		Deck.Remove( card );
	}

	public bool Add( Card card )
	{
		if ( !CanAdd( card ) )
		{
			return false;
		}

		Deck.Add( card );
		return true;
	}

	public bool CanAdd( Card card )
	{
		return Deck.Count < MaxCards;
	}

	public bool CanPlay( Card card )
	{
		if ( !Owner.IsValid() )
		{
			return false;
		}

		if ( !Hand.Contains( card ) )
		{
			return false;
		}

		if ( Owner.Energy < card.EffectiveCost.Ep )
		{
			return false;
		}

		if ( Owner.Mana < card.EffectiveCost.Mp )
		{
			return false;
		}

		return true;
	}
}
