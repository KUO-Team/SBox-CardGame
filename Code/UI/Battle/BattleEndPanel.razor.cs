using System;
using Sandbox.UI;

namespace CardGame.UI;

public partial class BattleEndPanel
{
	[Parameter]
	public bool IsWin { get; set; }
	
	private Faction WinningFaction => IsWin ? Faction.Player : Faction.Enemy;
	
	protected override void OnClick( MousePanelEvent e )
	{
		OnBattleEnd();
		base.OnClick( e );
	}
	
	private void OnBattleEnd()
	{
		GameManager.Instance?.OnBattleEnd( WinningFaction );
		this.Hide();
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( IsWin );
	}
}
