namespace CardGame.Effects;

public class PowerUp( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Gain @ Power Up";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.PowerUp, Power );
		base.OnPlay( detail );
	}
}
