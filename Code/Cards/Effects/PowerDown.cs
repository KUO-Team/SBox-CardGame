namespace CardGame.Effects;

public class PowerDown( Card card ) : CardEffect( card )
{
	public override string Description => "Inflict @ Power Down";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Target?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.PowerDown, Power.Value );
		base.OnPlay( detail );
	}
}
