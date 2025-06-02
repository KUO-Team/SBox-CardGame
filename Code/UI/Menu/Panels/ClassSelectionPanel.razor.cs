using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI;

public partial class ClassSelectionPanel
{
	private static PlayerClass? SelectedClass => Player.Local?.Class;
	
	private static GameManager? GameManager => GameManager.Instance;
	private static MapManager? MapManager => MapManager.Instance;
	private static SaveManager? SaveManager => SaveManager.Instance;
	private static RelicManager? RelicManager => RelicManager.Instance;
	private static SceneManager? SceneManager => SceneManager.Instance;
	
	private static void SelectClass( PlayerClass playerClass )
	{
		Player.Local?.SetClass( playerClass );
	}
	
	public void StartRun()
	{
		if ( !Menu.IsValid() )
		{
			return;
		}
		
		Menu.Relics();
	}
	
	public void Close()
	{
		this.Hide();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Player.Local?.Class );
	}
}
