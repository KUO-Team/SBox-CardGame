using Sandbox.UI;
using CardGame.UI.Tutorial;

namespace CardGame.UI;

public partial class PauseMenu
{
	private SettingsPanel? _settingsPanel;
	
	private static SaveManager? SaveManager => CardGame.SaveManager.Instance;
	private static SceneManager? SceneManager => CardGame.SceneManager.Instance;

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
		if ( !SceneManager.IsValid() )
		{
			return;
		}
		
		TutorialPanel.Instance?.Clear();
		BattleManager.Instance?.UnloadBattleScript();

		Scene.Load( SceneManager.MenuScene );
	}
}
