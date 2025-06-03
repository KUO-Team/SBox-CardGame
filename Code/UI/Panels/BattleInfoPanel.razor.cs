using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI;

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

		if ( BattleManager.Instance is not {} battleManager )
		{
			return;
		}

		battleManager?.StartBattle( Battle );
	}
	
	private UnitInfoPanel? _unitInfoPanel;
	
	public void ShowInfo( Battle.BattleUnit unit )
	{
		if ( !_unitInfoPanel.IsValid() )
		{
			return;
		}
		
		_unitInfoPanel.BattleUnit = unit;
		_unitInfoPanel.Show();
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( Battle );
	}
}
