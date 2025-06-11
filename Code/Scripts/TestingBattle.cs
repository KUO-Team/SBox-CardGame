namespace CardGame.Scripts;

public class TestingBattle : BattleScript
{
	private static BattleManager? BattleManager => BattleManager.Instance;
	private static int TurnCount => BattleManager?.Turn ?? 0;
	
	public override void OnTurnStart()
	{
		// Perform logic here.
		base.OnTurnStart();
	}
}
