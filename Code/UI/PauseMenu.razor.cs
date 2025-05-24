using System;
using Sandbox.UI;
using CardGame.UI.Tutorial;

namespace CardGame.UI;

public partial class PauseMenu
{
	private SettingsPanel? _settingsPanel;
	
	private static SaveManager? SaveManager => SaveManager.Instance;
	private static SceneManager? SceneManager => SceneManager.Instance;
	private static BattleManager? BattleManager => BattleManager.Instance;

	public void Continue()
	{
		PauseManager.Instance?.Unpause();
	}
	
	public void SaveAndQuit()
	{
		if ( !SaveManager.IsValid() )
		{
			return;
		}

		SaveManager.Save();
		ToMenu();
	}
	
	public void Settings()
	{
		_settingsPanel?.Show();
	}
	
	public void ToMenu()
	{
		TutorialPanel.Instance?.Clear();
		RelicManager.Instance?.ClearRelics();
		BattleManager.Instance?.UnloadBattleScript();
		SaveManager?.ClearActiveRun();
		SceneManager?.LoadScene( SceneManager.Scenes.Menu );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( BattleManager?.IsTutorial );
	}
}
