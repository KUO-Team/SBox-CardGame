using Sandbox.UI;
namespace CardGame.UI;

public partial class PauseMenu
{
	private SettingsPanel? _settingsPanel;
	private Panel? _main;
	
	private static SaveManager? SaveManager => CardGame.SaveManager.Instance;
	private static SceneManager? SceneManager => CardGame.SceneManager.Instance;
	
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

		Scene.Load( SceneManager.MenuScene );
	}
}
