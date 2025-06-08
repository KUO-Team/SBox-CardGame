namespace CardGame.Relics;

public class EchoOfOne( Data.Relic data ) : Relic( data )
{
	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() || !Owner.HealthComponent.IsValid() || !Owner.Slots.IsValid() )
		{
			return;
		}

		if ( Owner.HealthComponent.IsDead )
		{
			return;
		}

		if ( BattleManager.GetAliveUnits( Owner.Faction ).Count > 1 )
		{
			return;
		}
		
		Owner.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.AttackPowerUp, 3 );
		base.OnTurnStart();
	}
}
