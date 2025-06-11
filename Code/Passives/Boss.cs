namespace CardGame.Passives;

public class Boss( Data.PassiveAbility data ) : PassiveAbility( data )
{
	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() || !Owner.HandComponent.IsValid() )
		{
			return;
		}
		
		foreach ( var card in Owner.HandComponent.Deck.ToArray() )
		{
			Owner.HandComponent.Draw( card, true );
		}
		
		Owner.RecoverMana( Owner.MaxMana );
		
		base.OnTurnStart();
	}
}
