namespace CardGame.Effects;

public class Heal( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Heal @ HP";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.HealthComponent?.Heal( Power );
		base.OnPlay( detail );
	}
}
