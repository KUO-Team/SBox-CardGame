namespace CardGame.Effects;

public class DiscardRandom( Card card ) : CardEffect( card )
{
	public override string Description => Power == 1 ? "Discard 1 random card from hand" : "Discard @ random cards from hand";

	public override void OnPlay( CardEffectDetail detail )
	{
		var hand = detail.Unit?.HandComponent;
		if ( !hand.IsValid() )
		{
			return;
		}
		
		for ( var i = 0; i < Power; i++ )
		{
			var randomCard = Game.Random.FromList( hand.Hand! );
			if ( randomCard is not null )
			{
				hand.Discard( randomCard );
			}
		}
		
		base.OnPlay( detail );
	}
}
