namespace CardGame.Effects;

public class ManaRecover( Card card ) : CardEffect( card )
{
	public override string Description { get; set; } = "Recover @ MP";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.RecoverMana( Power.Value );
		base.OnPlay( detail );
	}
}
