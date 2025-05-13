using CardGame.Scripts;

namespace CardGame;

public sealed partial class BattleManager
{
	public BattleScript? BattleScript { get; set; }

	public void LoadBattleScript( string script )
	{
		BattleScript = TypeLibrary.Create<BattleScript>( script );
		BattleScript?.OnLoad();
	}

	public void UnloadBattleScript()
	{
		if ( BattleScript is null )
		{
			return;
		}

		BattleScript.OnUnload();
		BattleScript = null;
	}
}
