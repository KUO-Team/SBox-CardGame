namespace CardGame.Effects;

public class ManaRecover( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Recover @ MP";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		if ( !detail.Unit.IsValid() )
		{
			return;
		}

		detail.Unit.RecoverMana( EffectivePower );
		base.OnPlay( detail );
	}
}
