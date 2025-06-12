namespace CardGame.Effects;

public class Protection( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Gain @ Protection";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Protection, EffectivePower );
		base.OnPlay( detail );
	}
}
