namespace CardGame.Effects;

public class DrawDeck( Card card ) : CardEffect( card )
{
	public override string Description => "Draw your deck";

	public override void OnPlay( CardEffectDetail detail )
	{
		if ( !detail.Unit.IsValid() || !detail.Unit.HandComponent.IsValid() )
		{
			return;
		}

		foreach ( var card in detail.Unit.HandComponent.Deck )
		{
			detail.Unit.HandComponent.Draw( card );
		}
		base.OnPlay( detail );
	}
}
