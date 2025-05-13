namespace CardGame.Relics;

public class LoosePockets( Data.Relic data ) : Relic( data )
{
	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() )
		{
			return;
		}
		
		Owner.HandComponent?.DrawX( 2 );
		
		var randomCard = GetRandomCardInHand();
		if ( randomCard is not null )
		{
			Owner.HandComponent?.Discard( randomCard );
		}
		
		base.OnTurnStart();
	}

	private Card? GetRandomCardInHand()
	{
		if ( !Owner.IsValid() )
		{
			return null;
		}

		if ( !Owner.HandComponent.IsValid() )
		{
			return null;
		}

		return Game.Random.FromList( Owner.HandComponent.Hand! );
	}
}
