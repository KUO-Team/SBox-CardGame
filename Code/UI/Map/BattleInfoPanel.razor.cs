using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI.Map;

public partial class BattleInfoPanel
{
	[Parameter]
	public Battle? Battle { get; set; }
	
	private DeckbuildingPanel? _deckbuildingPanel;
	
	public void EditDeck()
	{
		if ( !_deckbuildingPanel.IsValid() )
		{
			return;
		}

		_deckbuildingPanel.UnitData = Player.Local?.Unit;
		_deckbuildingPanel?.Show();
	}
	
	public void StartBattle()
	{
		if ( Battle is null )
		{
			return;
		}

		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		var unit = player.Unit;
		if ( unit is null )
		{
			return;
		}

		
		WarningPanel? warning = null;
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
				{
					if ( BattleManager.Instance is not {} battleManager )
					{
						return;
					}

					battleManager?.StartBattle( Battle );
				}
				break;
		}
	}
	
	private UnitInfoPanel? _unitInfoPanel;
	
	public void ShowInfo( Unit unit )
	{
		if ( !_unitInfoPanel.IsValid() )
		{
			return;
		}
		
		_unitInfoPanel.Unit = unit;
		_unitInfoPanel.Show();
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( Battle );
	}
}
