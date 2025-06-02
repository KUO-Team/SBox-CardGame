namespace CardGame.Effects;

public class Heal( Card card ) : CardEffect( card )
{
	public override string Description => "Heal @ HP";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.HealthComponent?.Heal( Power );
		base.OnPlay( detail );
	}
}
