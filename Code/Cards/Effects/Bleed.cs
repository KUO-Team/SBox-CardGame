namespace CardGame.Effects;

public class Bleed( Card card ) : CardEffect( card )
{
	public override string Description => "Inflict @ Bleed";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Target?.StatusEffects?.AddStatusEffect<StatusEffects.Bleed>( Power.Value );
		base.OnPlay( detail );
	}
}
