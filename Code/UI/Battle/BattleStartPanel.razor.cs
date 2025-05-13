using System;
using Sandbox.UI;

namespace CardGame.UI;

public partial class BattleStartPanel
{
	private static BattleManager? BattleManager => BattleManager.Instance;

	public async void OnBattleStart()
	{
		try
		{
			this.Show();
			await Task.DelaySeconds( 2 );
			this.Hide();
		}
		catch ( Exception e )
		{
			Log.Warning( e );
		}
	}
}
