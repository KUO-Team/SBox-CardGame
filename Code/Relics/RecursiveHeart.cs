using CardGame.Units;

namespace CardGame.Relics;

public class RecursiveHeart( Data.Relic data ) : Relic( data )
{
	private readonly Dictionary<BattleUnitComponent, Card> _playedCards = new();

	public override void OnPlayCard( Card card, BattleUnitComponent unitComponent )
	{
		if ( !_playedCards.TryAdd( unitComponent, card ) )
		{
			return;
		}
		
		unitComponent.HandComponent?.Draw( card, true );
		base.OnPlayCard( card, unitComponent );
	}

	public override void OnTurnEnd()
	{
		_playedCards.Clear();
		base.OnTurnEnd();
	}
}
