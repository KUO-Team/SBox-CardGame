namespace CardGame.Effects;

public class Enchant( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Become Enchanted";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Enchanted );
		base.OnPlay( detail );
	}
}
