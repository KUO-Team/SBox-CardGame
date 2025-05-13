using CardGame.Units;

namespace CardGame.Relics;

public class RecursiveHeart( Data.Relic data ) : Relic( data )
{
	public override void AfterPlayCard( Card card, BattleUnit unit )
	{
		unit.HandComponent?.Draw( card, true );
		base.AfterPlayCard( card, unit );
	}
}
