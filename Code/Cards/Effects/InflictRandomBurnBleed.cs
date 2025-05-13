namespace CardGame.Effects;

public class InflictRandomBleedBurn( Card card ) : CardEffect( card )
{
	public override string Description => "Randomly inflict @ of the following: Bleed, Burn";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		var t = Game.Random.Float();
		if ( t > 0.5 )
		{
			detail.Target?.StatusEffects?.AddStatusEffect<StatusEffects.Bleed>( Power.Value );
		}
		else
		{
			detail.Target?.StatusEffects?.AddStatusEffect<StatusEffects.Burn>( Power.Value );
		}
		base.OnPlay( detail );
	}
}
