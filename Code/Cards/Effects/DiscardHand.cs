namespace CardGame.Effects;

public class DiscardHand( Card card ) : CardEffect( card )
{
	public override string Description => "Discard your hand";

	public override void OnPlay( CardEffectDetail detail )
	{
		var hand = detail.Unit?.HandComponent;
		if ( hand.IsValid() )
		{
			hand.DiscardHand();
		}
		
		base.OnPlay( detail );
	}
}
