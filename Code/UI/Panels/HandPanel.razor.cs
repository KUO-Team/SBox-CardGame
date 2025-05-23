using System;
using Sandbox.UI;
using CardGame.Data;
using CardGame.Units;

namespace CardGame.UI;

public partial class HandPanel
{
	[Parameter]
	public HandComponent? HandComponent { get; set; }
	
	public static Card? SelectedCard { get; set; }

	public static event Action<Card>? OnCardSelected;
	public static event Action<Card>? OnCardDeselected;
	
	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
		{
			return;
		}

		// Initial build of hand
		//RebuildHand();
		base.OnAfterTreeRender( firstTime );
	}

	protected override void OnParametersSet()
	{
		if ( HandComponent.IsValid() )
		{
			HandComponent.Panel = this;
		}
		
		base.OnParametersSet();
	}

	public override void OnDeleted()
	{
		foreach ( var @delegate in OnCardSelected?.GetInvocationList() ?? [] )
		{
			OnCardSelected -= @delegate as Action<Card>;
		}

		foreach ( var @delegate in OnCardDeselected?.GetInvocationList() ?? [] )
		{
			OnCardDeselected -= @delegate as Action<Card>;
		}

		base.OnDeleted();
	}

	public void SelectCard( Card card )
	{
		if ( SelectedCard is not null && SelectedCard == card )
		{
			OnCardDeselected?.Invoke( card );
			SelectedCard = null;
		}
		else
		{
			OnCardSelected?.Invoke( card );
			SelectedCard = card;
		}
	}
	
	public bool IsInDiscardMode { get; set; }

	public void EnterDiscardMode()
	{
		IsInDiscardMode = true;
		this.Show();
	}

	public void LeaveDiscardMode()
	{
		IsInDiscardMode = false;
	}

	public void ConfirmDiscard()
	{
		HandComponent?.ConfirmDiscard();
	}

	private Card? _draggingCard;
	private Vector2 _dragStartMousePos;
	private Vector2 _dragOffset;
	private int _hoverInsertIndex = -1;
	private bool _isDragging = false;
	private float _cardSpacing = 10f; // Spacing between cards in normal state
	private float _hoverGapSize = 30f; // Size of gap when hovering between cards

	protected override void OnMouseUp( MousePanelEvent e )
	{
		// Handle card release
		FinishDrag();
		base.OnMouseUp( e );
	}

	public void OnDragStart( PanelEvent e, Card card )
	{
		var m = Mouse.Position;

		_draggingCard = card;
		_dragStartMousePos = m;
		_isDragging = true;

		var cardPanel = FindCardPanel( card );
		if ( cardPanel is null )
		{
			return;
		}
		
		var panelPos = PanelPositionToScreenPosition( cardPanel.Box.Rect.Position );
		_dragOffset = _dragStartMousePos - panelPos;

		// Bring the card to the front
		cardPanel.Style.ZIndex = 10;
		cardPanel.Style.Dirty();
	}

	private void FinishDrag()
	{
		if ( !_isDragging || _draggingCard is null )
		{
			return;
		}

		if ( HandComponent?.Hand is {} hand )
		{
			var currentIndex = hand.IndexOf( _draggingCard );
			if ( currentIndex >= 0 )
			{
				hand.RemoveAt( currentIndex );
			}

			var maxInsertIndex = hand.Count;
			if ( _hoverInsertIndex < 0 )
			{
				_hoverInsertIndex = 0;
			}
			if ( _hoverInsertIndex > maxInsertIndex )
			{
				_hoverInsertIndex = maxInsertIndex;
			}

			hand.Insert( _hoverInsertIndex, _draggingCard );
		}

		_draggingCard = null;
		_isDragging = false;
		_hoverInsertIndex = -1;

		RebuildHand();
	}

	private int GetInsertIndex( Vector2 mousePos )
	{
		// Safe check for dragging card
		if ( _draggingCard == null )
		{
			return 0;
		}

		var panels = Children.OfType<CardPanel>().Where( p => p.Card != _draggingCard ).ToList();
		if ( panels.Count == 0 )
		{
			return 0;
		}

		// Check if mouse is to the left of the first card
		var firstPanel = panels.FirstOrDefault();
		if ( firstPanel != null )
		{
			var firstRect = firstPanel.Box.Rect;
			var firstPos = PanelPositionToScreenPosition( firstRect.Position );

			if ( mousePos.x < firstPos.x )
			{
				return 0;
			}
		}

		// Check positions between cards
		for ( var i = 0; i < panels.Count - 1; i++ )
		{
			var leftPanel = panels[i];
			var rightPanel = panels[i + 1];

			var leftRect = leftPanel.Box.Rect;
			var rightRect = rightPanel.Box.Rect;

			var leftPos = PanelPositionToScreenPosition( leftRect.Position );
			var rightPos = PanelPositionToScreenPosition( rightRect.Position );

			var leftEdge = leftPos.x + leftRect.Width;
			var rightEdge = rightPos.x;

			var midpoint = (leftEdge + rightEdge) / 2;

			if ( mousePos.x < midpoint )
			{
				return i + 1;
			}
		}

		// If we get here, insert after the last card
		return panels.Count;
	}

	public override void Tick()
	{
		base.Tick();

		// Safety check for empty hand
		if ( HandComponent?.Hand == null || HandComponent.Hand.Count == 0 )
		{
			if ( !_isDragging )
			{
				return;
			}
			
			_isDragging = false;
			_draggingCard = null;
			return;
		}

		if ( !_isDragging || _draggingCard is null )
		{
			return;
		}

		var mouse = Mouse.Position;

		// Get all card panels (safely)
		var allPanels = Children.OfType<CardPanel>().ToList();
		if ( allPanels.Count == 0 )
		{
			_isDragging = false;
			_draggingCard = null;
			return;
		}

		var draggedPanel = allPanels.FirstOrDefault( p => p.Card == _draggingCard );
		if ( draggedPanel is null )
		{
			_isDragging = false;
			_draggingCard = null;
			return;
		}

		// Calculate insert index safely
		_hoverInsertIndex = GetInsertIndex( mouse );

		// Get non-dragged panels
		var panels = allPanels.Where( p => p != draggedPanel ).ToList();

		// Calculate base positions for all other cards
		float baseX = 0;
		var visibleIndex = 0;

		foreach ( var panel in panels )
		{
			// Add gap for insertion point
			if ( visibleIndex == _hoverInsertIndex )
			{
				baseX += _hoverGapSize;
			}

			// Set panel position
			panel.Style.Position = PositionMode.Absolute;
			panel.Style.Left = Length.Pixels( baseX );
			panel.Style.Top = Length.Pixels( 0 );
			panel.Style.ZIndex = 1;
			panel.Style.Dirty();

			// Move to next position
			baseX += panel.Box.Rect.Width + _cardSpacing;
			visibleIndex++;
		}

		// Handle special case: insert at the end
		if ( _hoverInsertIndex >= panels.Count && panels.Count > 0 )
		{
			var lastPanel = panels.LastOrDefault();
			if ( lastPanel.IsValid() )
			{
				if ( lastPanel.Style.Left.HasValue )
				{
					var lastPanelRight = lastPanel.Style.Left.Value.GetPixels( 100 ) + lastPanel.Box.Rect.Width;
					baseX = lastPanelRight + _hoverGapSize;
				}
			}
		}

		// Position the dragged card
		draggedPanel.Style.Position = PositionMode.Absolute;
		draggedPanel.Style.Left = Length.Pixels( mouse.x - _dragOffset.x );
		draggedPanel.Style.Top = Length.Pixels( mouse.y - _dragOffset.y );
		draggedPanel.Style.ZIndex = 10;
		draggedPanel.Style.Dirty();
	}

	private void RebuildHand()
	{
		// Remove all existing card panels
		DeleteChildren( true );

		// Safety check
		if ( HandComponent?.Hand is null )
			return;

		// Re-add them in current order
		float xPos = 0;

		foreach ( var card in HandComponent.Hand )
		{
			var panel = new CardPanel
			{
				Card = card
			};

			panel.AddClass( "card" );
			if ( SelectedCard == card )
			{
				panel.AddClass( "selected" );
			}

			// Position cards horizontally
			panel.Style.Position = PositionMode.Absolute;
			panel.Style.Left = Length.Pixels( xPos );
			panel.Style.Top = Length.Pixels( 0 );
			panel.Style.ZIndex = 1;

			// Setup event handlers
			//panel.AddEventListener( "onmousedown", e => OnDragStart( e, card ) );
			//panel.AddEventListener( "onclick", () => SelectCard( card ) );

			// Add to parent
			AddChild( panel );

			// Once added, we can get its width for positioning the next card
			xPos += panel.Box.Rect.Width + _cardSpacing;

			// Add keywords if any
			if ( card.Keywords.Any() )
			{
				var keywordsPanel = new Panel();
				keywordsPanel.AddClass( "keywords" );
				keywordsPanel.AddClass( "hidden" );

				foreach ( var keywordString in card.Keywords )
				{
					var keyword = KeywordLookup.FindKeyword( keywordString );

					if ( keyword is null )
					{
						continue;
					}

					var keywordPanel = new KeywordPanel( keyword );
					keywordsPanel.AddChild( keywordPanel );
				}

				AddChild( keywordsPanel );
			}
		}
	}

	private Panel? FindCardPanel( Card card )
	{
		return Children.FirstOrDefault( p => p is CardPanel cp && cp.Card == card );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( HandComponent, HandComponent?.Hand.Count, HandComponent?.Deck.Count, SelectedCard, IsInDiscardMode, InputComponent.SelectedSlot );
	}
}
