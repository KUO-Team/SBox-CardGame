using System;
using Sandbox.UI;

namespace CardGame.UI;

public partial class TurnStartPanel
{
	private static BattleManager? BattleManager => BattleManager.Instance;

	public bool DontShow { get; set; }

	private bool _isShowing;

	protected override void OnClick( MousePanelEvent e )
	{
		base.OnClick( e );

		// Only hide if currently showing
		if ( _isShowing )
		{
			_isShowing = false;
			this.Hide();
		}
	}

	public async void OnTurnStart()
	{
		try
		{
			if ( DontShow )
				return;

			_isShowing = true;
			this.Show();

			var elapsed = 0f;
			const float duration = 2f;
			const float interval = 0.1f;

			while ( elapsed < duration && _isShowing )
			{
				await Task.DelaySeconds( interval );
				elapsed += interval;
			}

			this.Hide();
			_isShowing = false;
		}
		catch ( Exception e )
		{
			Log.Warning( e );
		}
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( BattleManager?.Turn );
	}
}
