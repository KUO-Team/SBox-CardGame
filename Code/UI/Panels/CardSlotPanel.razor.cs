using System;
using Sandbox.UI;

namespace CardGame.UI;

public partial class CardSlotPanel
{
	[Parameter]
	public CardSlot? Slot { get; set; }
	
	private bool IsAssigned => Slot?.AssignedCard is not null;
	
	private bool IsAvailable => Slot?.IsAvailable ?? false;

	public void Init()
	{
		if ( Slot.IsValid() )
		{
			Slot.Panel = this;
		}
	}

	protected override void OnClick( MousePanelEvent e )
	{
		if ( !Slot.IsValid() )
		{
			return;
		}

		if ( !BattleManager.Instance.IsValid() )
		{
			return;
		}

		if ( BattleManager.Instance.State != TurnState.Start )
		{
			return;
		}

		var selectedSlot = InputComponent.SelectedSlot;
		var selectedCard = HandPanel.SelectedCard;

		// Deselect the currently selected slot if clicking it again
		if ( selectedSlot == Slot )
		{
			Slot.ClearTargetingArrows();
			InputComponent.SelectedSlot = null;
			return;
		}

		// If a card is selected and the user is clicking a target slot
		if ( selectedCard is not null && selectedSlot is not null )
		{
			if ( !selectedSlot.CanAssignCard( selectedCard, Slot ) )
			{
				return;
			}

			if ( !CanTarget( selectedCard ) )
			{
				return;
			}

			selectedSlot.AssignCard( selectedCard, Slot );
			return;
		}

		// If no card is selected, allow selecting this slot (must be a player faction)
		if ( IsPlayerFaction() )
		{
			HandPanel.SelectedCard = null;
			InputComponent.SelectedSlot = Slot;
			InputComponent.OnSelect?.Invoke( Slot );
		}

		base.OnClick( e );
	}
	
	protected override void OnRightClick( MousePanelEvent e )
	{
		if ( IsAssigned && CanInteract() )
		{
			Slot?.ClearTargetingArrows();
			Slot?.UnassignCard();
		}

		base.OnRightClick( e );
	}
	
	protected override void OnMouseOver( MousePanelEvent e )
	{
		if ( !Slot.IsValid() )
		{
			return;
		}

		if ( BattleManager.Instance is not {} battleManager )
		{
			return;
		}
		
		// Show assigned card info on hover for any slot
		if ( IsAssigned )
		{
			var hud = battleManager.Hud;
			if ( hud.IsValid() && Slot.IsAssigned )
			{
				hud.ShowCard( Slot.AssignedCard! );
			}
		}

		if ( battleManager.State != TurnState.Start )
		{
			return;
		}

		// Show hand panel on hover only for interactable slots with no active selection
		var battleHud = BattleHud.Instance;
		if ( battleHud.IsValid() && CanInteract() && InputComponent.SelectedSlot == null )
		{
			var handPanel = battleHud.Panel.ChildrenOfType<HandPanel>().First();
			handPanel?.Show();
		}

		base.OnMouseOver( e );
	}
	
	protected override void OnMouseOut( MousePanelEvent e )
	{
		if ( !Slot.IsValid() )
		{
			return;
		}

		if ( BattleManager.Instance is not {} battleManager )
		{
			return;
		}

		if ( battleManager.State != TurnState.Start )
		{
			return;
		}

		var hud = battleManager.Hud;
		if ( hud.IsValid() )
		{
			hud.RemoveCard();
		}

		var battleHud = BattleHud.Instance;
		if ( battleHud.IsValid() )
		{
			// Only hide hand if we're interactable and no slot is selected
			if ( CanInteract() && InputComponent.SelectedSlot == null )
			{
				var handPanel = battleHud.Panel.ChildrenOfType<HandPanel>().First();
				handPanel?.Hide();
			}
		}

		base.OnMouseOut( e );
	}
	
	public bool CanTarget( Card card )
	{
		if ( !Slot.IsValid() || !Slot.Owner.IsValid() )
		{
			return false;
		}

		if ( !Player.Local.IsValid() )
		{
			return false;
		}

		var unit = Slot.Owner;
		var player = Player.Local.Units.FirstOrDefault();

		// Check Self
		if ( card.Targeting.HasFlag( Card.CardTargets.Self ) && unit == player )
		{
			return true;
		}

		// Check Ally (same faction, but not self)
		if ( card.Targeting.HasFlag( Card.CardTargets.Ally ) && unit.Faction == Faction.Player && unit != player )
		{
			return true;
		}

		// Check Enemy (not same faction)
		return card.Targeting.HasFlag( Card.CardTargets.Enemy ) && unit.Faction != Faction.Player;
	}
	
	/// <summary>
	/// If the player is able to interact with this card slot (select it)
	/// </summary>
	public bool CanInteract()
	{
		return IsPlayerFaction();
	}
	
	private bool IsPlayerFaction()
	{
		if ( !Slot.IsValid() || !Slot.Owner.IsValid() )
		{
			return false;
		}

		return Slot.Owner.Faction == Faction.Player;
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( IsAssigned, IsAvailable, Slot, Slot?.AssignedCard, Slot?.Speed,  BattleManager.Instance?.Turn, BattleManager.Instance?.State, InputComponent.SelectedSlot );
	}
}
