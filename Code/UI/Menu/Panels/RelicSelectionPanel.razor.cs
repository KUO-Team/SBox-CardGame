using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI;

public partial class RelicSelectionPanel
{
	public List<Relic> Relics { get; set; } = [];

	public List<Relic> SelectedRelics { get; private set; } = [];

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
	}
	
	public void ToggleRelicSelection( Relic relic, bool deselectOld = true )
	{
		if ( !RelicManager.IsValid() )
		{
			return;
		}
		
		if ( deselectOld && MaxSelectableRelics == 1 )
		{
			foreach ( var selectedRelic in SelectedRelics.ToList().Where( selectedRelic => selectedRelic != relic ) )
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
		
		if ( SelectedRelics.Count >= MaxSelectableRelics )
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

	public void Close()
	{
		this.Hide();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( SelectedRelics.Count );
	}
}
