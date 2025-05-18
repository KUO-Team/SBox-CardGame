namespace CardGame.Relics;

public class EchoOfOne( Data.Relic data ) : Relic( data )
{
	private bool _added;
	
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
		
		Owner.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.PowerUp, 3 );

		if ( _added )
		{
			return;
		}

		Owner.Slots.AddCardSlot( 1 );
		_added = true;
		
		base.OnTurnStart();
	}
}
