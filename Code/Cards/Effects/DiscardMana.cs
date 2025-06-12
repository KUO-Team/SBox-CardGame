namespace CardGame.Effects;

public class DiscardMana( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "If this card is discarded, recover @ MP";

	public override void OnDiscard( CardEffectDetail detail )
	{
		if ( !detail.Unit.IsValid() )
		{
			return;
		}

		detail.Unit.RecoverMana( EffectivePower );
		base.OnDiscard( detail );
	}
}
