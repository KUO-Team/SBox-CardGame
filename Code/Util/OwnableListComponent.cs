﻿using CardGame.Units;

namespace CardGame;

public abstract class OwnableListComponent<T> : ListComponent<T>, IOwnable
{
	[Property, RequireComponent, Order( -1 )]
	public BattleUnit? Owner { get; set; }

	protected override void OnStart()
	{
		if ( Components.TryGet( out BattleUnit unit ) )
		{
			Owner = unit;
		}

		base.OnStart();
	}
}
