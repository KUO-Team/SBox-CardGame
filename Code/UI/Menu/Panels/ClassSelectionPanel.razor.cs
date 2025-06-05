using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI;

public partial class ClassSelectionPanel
{
	private List<PlayerClass> PlayerClasses { get; set; } = [];
	
	private static PlayerClass? SelectedClass => Player.Local?.Class;
	
	private static GameManager? GameManager => GameManager.Instance;
	private static MapManager? MapManager => MapManager.Instance;
	private static SaveManager? SaveManager => SaveManager.Instance;
	private static RelicManager? RelicManager => RelicManager.Instance;
	private static SceneManager? SceneManager => SceneManager.Instance;

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
		{
			return;
		}
		
		PlayerClasses.Clear();
		foreach ( var @class in PlayerClassDataList.All )
		{
			var copy = @class.DeepCopy();
			PlayerClasses.Add( copy );
		}

		var firstClass = PlayerClasses[0];
		SelectClass( firstClass );
		
		base.OnAfterTreeRender( firstTime );
	}

	private static void SelectClass( PlayerClass playerClass )
	{
		Player.Local?.SetClass( playerClass );
	}

	private void AddRelic( Relic relic )
	{
		var relicSelectionPanel = GetMenuSubPanel<RelicSelectionPanel>();
		if ( relicSelectionPanel.IsValid() )
		{
			relicSelectionPanel.SelectedRelics.Add( relic );
		}
	}
	
	private void ClearRelics()
	{
		var relicSelectionPanel = GetMenuSubPanel<RelicSelectionPanel>();
		if ( relicSelectionPanel.IsValid() )
		{
			relicSelectionPanel.SelectedRelics.Clear();
		}
	}
	
	public void StartRun()
	{
		if ( !Menu.IsValid() )
		{
			return;
		}
		
		if ( SelectedClass is null )
		{
			return;
		}
		
		ClearRelics();
		foreach ( var relicId in SelectedClass.Relics )
		{
			var relic = RelicDataList.GetById( relicId );
			if ( relic is null )
			{
				continue;
			}
			
			AddRelic( relic );
		}
		
		Menu.Relics();
	}
	
	public void Close()
	{
		this.Hide();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Player.Local?.Class, SelectedClass );
	}
}
