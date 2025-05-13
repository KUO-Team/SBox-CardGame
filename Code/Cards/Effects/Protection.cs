namespace CardGame.Effects;

public class Protection( Card card ) : CardEffect( card )
{
	public override string Description => "Gain @ Protection";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.StatusEffects?.AddStatusEffect<StatusEffects.Protection>( Power.Value );
		base.OnPlay( detail );
	}
}
