namespace CardGame.Effects;

public class PowerUp( Card card ) : CardEffect( card )
{
	public override string Description => "Gain @ Power Up";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.PowerUp, Power.Value );
		base.OnPlay( detail );
	}
}
