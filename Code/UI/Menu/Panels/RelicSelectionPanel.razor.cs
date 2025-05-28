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

		if ( SelectedRelic == relic )
		{
			SelectedRelic = null;
			return;
		}

		SelectedRelic = relic;
	}

	public void StartRun()
	{
		if ( !Menu.IsValid() )
		{
			return;
		}
		
		if ( SelectedRelic is not null )
		{
			Menu.StartRun( [SelectedRelic] );
		}
		else
		{
			Menu.StartRun();
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
