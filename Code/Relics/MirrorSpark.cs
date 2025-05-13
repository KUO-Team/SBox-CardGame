using CardGame.Units;

namespace CardGame.Relics;

public class MirrorSpark( Data.Relic data ) : Relic( data )
{
	public override void OnPlayCard( Card card, BattleUnit unit )
	{
		if ( !Owner.IsValid() || card.Type != Card.CardType.Spell )
		{
			return;
		}

		// Use a copy of the actions list to avoid modifying it during iteration
		var originalActions = card.Actions.ToList();

		foreach ( var action in originalActions )
		{
			if ( action.Type != Action.ActionType.Effect )
			{
				continue;
			}

			if ( !ShouldDuplicate() )
			{
				continue;
			}

			var duplicated = action.DeepCopy();

			// 50% chance to randomize targeting
			if ( ShouldRandomizeTarget() )
			{
				// Just pretend your game isn’t going to explode when you randomize like this
				var randomTarget = BattleManager.AliveUnits
					.Select( x => x.Slots )
					.OrderBy( _ => Game.Random.Next() )
					.Select( x => Game.Random.FromList( x!.ToList()! ) )
					.OrderBy( _ => Game.Random.Next() )
					.FirstOrDefault();

				if ( randomTarget is not null )
				{
					//action.Target = randomTarget;
				}
			}

			card.Actions.Add( duplicated );
		}

		base.OnPlayCard( card, unit );
	}

	private static bool ShouldDuplicate()
	{
		return Game.Random.Next() % 2 == 0;
	}

	private static bool ShouldRandomizeTarget()
	{
		return Game.Random.Next() % 2 == 0;
	}
}
