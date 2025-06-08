namespace CardGame.Relics;

public class FatesFavor( Data.Relic data ) : Relic( data )
{
	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() || !Owner.HandComponent.IsValid() || !Owner.StatusEffects.IsValid() )
		{
			return;
		}

		var cardsInHand = Owner.HandComponent.Hand.Count;
		if ( cardsInHand != 0 )
		{
			return;
		}
		
		Owner.StatusEffects.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Protection, 2 );
		base.OnTurnStart();
	}
}
