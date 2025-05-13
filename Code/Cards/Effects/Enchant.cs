namespace CardGame.Effects;

public class Enchant( Card card ) : CardEffect( card )
{
	public override string Description => "Become Enchanted";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.StatusEffects?.AddStatusEffect<StatusEffects.Enchanted>();
		base.OnPlay( detail );
	}
}
