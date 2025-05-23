namespace CardGame.Relics;

public class Masquerade( Data.Relic data ) : Relic( data )
{
	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() || !Owner.StatusEffects.IsValid() )
		{
			return;
		}

		foreach ( var effect in Owner.StatusEffects )
		{
			if ( !effect.Data.IsNegative )
			{
				continue;
			}
			
			effect.Stack /= 2;
			if ( effect.Stack <= 0 )
			{
				effect.Destroy();
			}
		}

		if ( Game.Random.Int( 1 ) == 0 )
		{
			Owner.StatusEffects.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.PowerUp );
		}
		else
		{
			Owner.StatusEffects.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Protection );
		}
		
		base.OnTurnStart();
	}
}
