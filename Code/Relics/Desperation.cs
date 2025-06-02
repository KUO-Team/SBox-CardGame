using CardGame.Modifiers;

namespace CardGame.Relics;

public class Desperation( Data.Relic data ) : Relic( data )
{
	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() || !Owner.HandComponent.IsValid() )
		{
			return;
		}

		var isLowHp = IsLowHp();
		if ( !isLowHp )
		{
			return;
		}

		foreach ( var card in Owner.HandComponent.Hand )
		{
			card.Modifiers.AddModifier( new CostModifier( 0, -1, 1 ) );
		}
		
		base.OnTurnStart();
	}

	private bool IsLowHp()
	{
		if ( !Owner.IsValid() || !Owner.HealthComponent.IsValid() )
		{
			return false;
		}

		var healthPercent = (float)Owner.HealthComponent.Health / Owner.HealthComponent.MaxHealth;
		return healthPercent < 0.2f;
	}
}
