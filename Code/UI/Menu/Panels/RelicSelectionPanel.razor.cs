using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI;

public partial class RelicSelectionPanel
{
	public List<Relic> Relics { get; set; } = [];

	public Relic? SelectedRelic { get; set; }

	private static RelicManager? RelicManager => RelicManager.Instance;

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
		{
			return;
		}

		var commonRelics = RelicDataList.All
			.Where( x => x.IsAvailable )
			.Where( x => x.Rarity == Relic.RelicRarity.Common );
		
		var randomCommonRelics = commonRelics.OrderBy( _ => Game.Random.Next() ).Take( 3 ).ToList();
		Relics.AddRange( randomCommonRelics );

		base.OnAfterTreeRender( firstTime );
	}

	public void SelectRelic( Relic relic )
	{
		if ( !RelicManager.IsValid() )
		{
			return;
		}

		if ( SelectedRelic == relic )
		{
			SelectedRelic = null;
			return;
		}

		SelectedRelic = relic;
	}

	public void StartRun()
	{
		if ( SelectedRelic is not null )
		{
			MainMenu.StartRun( [SelectedRelic] );
		}
		else
		{
			MainMenu.StartRun();
		}
	}

	public void Close()
	{
		this.Hide();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( SelectedRelic );
	}
}
