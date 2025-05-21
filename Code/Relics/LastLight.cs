using CardGame.Data;
using CardGame.Units;

namespace CardGame.Relics;

public class LastLight( Data.Relic data ) : Relic( data )
{
	private bool _triggered;
	
	public override void OnTakeDamage( int damage, BattleUnit target, BattleUnit? attacker = null )
	{
		if ( target.Faction != Faction.Player )
		{
			return;
		}
		
		if ( !target.HealthComponent.IsValid() )
		{
			return;
		}
		
		if ( _triggered )
		{
			return;
		}

		if ( damage >= target.HealthComponent.Health )
		{
			var halfOfMax = target.HealthComponent.MaxHealth / 2;
			target.HealthComponent.Health = halfOfMax;
			_triggered = true;
		}
		
		base.OnTakeDamage( damage, target, attacker );
	}

	public override void OnBattleStart( Battle battle )
	{
		_triggered = false;
		base.OnBattleStart( battle );
	}
}
