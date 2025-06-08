using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI;

public partial class RelicSelectionPanel
{
	public List<Relic> Relics { get; set; } = [];

	public List<Relic> SelectedRelics { get; private set; } = [];
	
	private List<Relic> _playerSelectedRelics = [];

	/// <summary>
	/// How many relics can be selected at once.
	/// </summary>
	public int MaxSelectableRelics { get; set; } = 1;

	private static RelicManager? RelicManager => RelicManager.Instance;

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
		{
			return;
		}

		SetRandomRelics();
		base.OnAfterTreeRender( firstTime );
	}

	public void SetRandomRelics( int count = 3, Relic.RelicRarity rarity = Relic.RelicRarity.Common )
	{
		Relics.Clear();
		var commonRelics = RelicDataList.All
			.Where( x => x.IsAvailable )
			.Where( x => x.Rarity == rarity );
		
		var randomCommonRelics = commonRelics.OrderBy( _ => Game.Random.Next() ).Take( count ).ToList();
		Relics.AddRange( randomCommonRelics );
	}

	public void SelectRelic( Relic relic )
	{
		if ( !RelicManager.IsValid() )
		{
			return;
		}

		if ( !CanSelectRelic( relic ) )
		{
			return;
		}
		
		SelectedRelics.Add( relic );
		_playerSelectedRelics.Add( relic );
	}
	
	public void DeselectRelic( Relic relic )
	{
		if ( !RelicManager.IsValid() )
		{
			return;
		}

		if ( !IsRelicSelected( relic ) )
		{
			return;
		}
		
		SelectedRelics.Remove( relic );
		_playerSelectedRelics.Remove( relic );
	}
	
	public void ToggleRelicSelection( Relic relic, bool deselectOld = true )
	{
		if ( !RelicManager.IsValid() )
		{
			return;
		}
		
		var selectedRelics = SelectedRelics.ToList();
		if ( deselectOld && MaxSelectableRelics == 1 )
		{
			var relicsToDeselect = selectedRelics.Where( selectedRelic => selectedRelic != relic && Relics.Contains( selectedRelic ) ).ToList();
			foreach ( var selectedRelic in relicsToDeselect )
			{
				DeselectRelic( selectedRelic );
			}
		}

		if ( IsRelicSelected( relic ) )
		{
			DeselectRelic( relic );
		}
		else
		{
			if ( CanSelectRelic( relic ) )
			{
				SelectRelic( relic );
			}
		}
	}
	
	public bool CanSelectRelic( Relic relic )
	{
		if ( !RelicManager.IsValid() )
		{
			return false;
		}

		if ( SelectedRelics.Contains( relic ) )
		{
			return false;
		}
		
		if ( _playerSelectedRelics.Count >= MaxSelectableRelics )
		{
			return false;
		}

		return true;
	}
	
	public bool IsRelicSelected( Relic relic )
	{
		if ( !RelicManager.IsValid() )
		{
			return false;
		}

		return SelectedRelics.Contains( relic );
	}

	public void StartRun()
	{
		if ( !Menu.IsValid() )
		{
			return;
		}
		
		Menu.StartRun( SelectedRelics );
	}

	private void Close()
	{
		SelectedRelics.Clear();
		_playerSelectedRelics.Clear();
		this.Hide();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( SelectedRelics.Count );
	}
}
