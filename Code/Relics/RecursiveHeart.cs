using CardGame.Units;

namespace CardGame.Relics;

public class RecursiveHeart( Data.Relic data ) : Relic( data )
{
	private readonly Dictionary<BattleUnit, Card> _playedCards = new();
	
	public override void AfterPlayCard( Card card, BattleUnit unit )
	{
		if ( !_playedCards.TryAdd( unit, card ) )
		{
			return;
		}
		
		unit.HandComponent?.Draw( card, true );
		base.AfterPlayCard( card, unit );
	}

	public override void OnTurnEnd()
	{
		_playedCards.Clear();
		base.OnTurnEnd();
	}
}
