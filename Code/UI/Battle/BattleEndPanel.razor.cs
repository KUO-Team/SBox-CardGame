using System;
using CardGame.Data;
using Sandbox.UI;

namespace CardGame.UI;

public partial class BattleEndPanel
{
	[Parameter]
	public bool IsWin { get; set; }
	
	public BattleRewards? RewardData { get; set; }
	
	public RewardsPanel? Rewards { get; set; }
	
	private Faction WinningFaction => IsWin ? Faction.Player : Faction.Enemy;

	public void OnWin( BattleRewards rewards )
	{
		RewardData = rewards;
		this.Show();
	}
	
	public void Close()
	{
		GameManager.Instance?.OnBattleEnd( WinningFaction );
		this.Hide();
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( IsWin );
	}
}
