namespace CardGame.Effects;

public class PowerUp( Card card ) : CardEffect( card )
{
	public override string Description => "Gain @ Power Up";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.StatusEffects?.AddStatusEffect<StatusEffects.PowerUp>( Power.Value );
		base.OnPlay( detail );
	}
}
