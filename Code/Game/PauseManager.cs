using Sandbox.Diagnostics;
using CardGame.UI;

namespace CardGame;

public sealed class PauseManager : Singleton<PauseManager>
{
	[Property]
	public PauseMenu? PauseMenu { get; set; }
	
	public static bool IsPaused => Game.ActiveScene.TimeScale == 0;
	
	private static readonly Logger Log = new( "PauseManager" );

	protected override void OnUpdate()
	{
		if ( Input.EscapePressed )
		{
			Input.EscapePressed = false;
			TogglePause();
		}
		
		base.OnUpdate();
	}

	public void Pause()
	{
		Scene.TimeScale = 0;
		
		if ( PauseMenu.IsValid() )
		{
			PauseMenu.Enabled = true;
		}
	}
	
	public void Unpause()
	{
		Scene.TimeScale = 1;
		
		if ( PauseMenu.IsValid() )
		{
			PauseMenu.Enabled = false;
		}
	}
	
	public void TogglePause()
	{
		if ( IsPaused )
		{
			Unpause();
		}
		else
		{
			Pause();
		}
	}
}
