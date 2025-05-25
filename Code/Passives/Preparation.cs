namespace CardGame.Passives;

public class Preparation( Data.PassiveAbility data ) : PassiveAbility( data )
{
	private static int TurnCount => BattleManager.Instance?.Turn ?? 0;

	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() || !Owner.HandComponent.IsValid() )
		{
			return;
		}

		if ( TurnCount == 1 )
		{
			Owner.HandComponent.DrawX( 2 );
		}

		base.OnTurnStart();
	}
}
