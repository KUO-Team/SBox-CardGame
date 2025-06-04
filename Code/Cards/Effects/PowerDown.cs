namespace CardGame.Effects;

public class PowerDown( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Inflict @ Power Down";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Target?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.PowerDown, Power );
		base.OnPlay( detail );
	}
}
