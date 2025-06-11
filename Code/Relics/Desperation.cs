using CardGame.Modifiers;

namespace CardGame.Relics;

public class Desperation( Data.Relic data ) : Relic( data )
{
	private List<Mod> _modifiers = [];

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
			var modifier = new CostModifier( -1, 1 );
			var mod = new Mod
			{
				Card = card, Modifier = modifier
			};
			
			card.Modifiers.AddModifier( modifier );
			_modifiers.Add( mod );
		}

		base.OnTurnStart();
	}

	public override void OnTurnEnd()
	{
		if ( !Owner.IsValid() || !Owner.HandComponent.IsValid() )
		{
			return;
		}

		foreach ( var card in Owner.HandComponent.Hand )
		{
			var mod = _modifiers.FirstOrDefault( x => x.Card == card );
			if ( mod is null )
			{
				continue;
			}

			card.Modifiers.RemoveModifier( mod.Modifier );
			_modifiers.Remove( mod );
		}
		
		base.OnTurnEnd();
	}

	private bool IsLowHp()
	{
		if ( !Owner.IsValid() || !Owner.HealthComponent.IsValid() )
		{
			return false;
		}

		var healthPercent = (float)Owner.HealthComponent.Health / Owner.HealthComponent.MaxHealth;
		return healthPercent < 0.35f;
	}

	private class Mod
	{
		public required CostModifier Modifier { get; set; }
		public required Card Card { get; set; }
	}
}
