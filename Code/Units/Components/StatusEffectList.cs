using System;
using CardGame.StatusEffects;

namespace CardGame.Units;

public class StatusEffectList : OwnableListComponent<StatusEffect>
{
	public IReadOnlyList<QueuedStatusEffect> NextTurnItems => _nextTurnItems.AsReadOnly();
	private readonly List<QueuedStatusEffect> _nextTurnItems = [];

	public void AddStatusEffect<T>( int stack = 1 ) where T : StatusEffect, new()
	{
		var statusEffect = new T();
		AddOrUpdate( statusEffect, stack );
	}

	public void AddStatusEffect( StatusEffect statusEffect, int stack = 1 )
	{
		AddOrUpdate( statusEffect, stack );
	}

	public void AddStatusEffectNextTurn<T>( int stack = 1 ) where T : StatusEffect, new()
	{
		var statusEffect = new T();
		AddOrUpdateQueued( statusEffect, stack );
	}

	public void AddStatusEffectNextTurn( StatusEffect statusEffect, int stack = 1 )
	{
		AddOrUpdateQueued( statusEffect, stack );
	}

	public bool HasStatusEffect<T>() where T : StatusEffect
	{
		return Items.Any( x => x is T );
	}
	
	public bool HasQueuedStatusEffect<T>() where T : StatusEffect
	{
		return _nextTurnItems.Any( x => x.StatusEffect is T );
	}

	private void AddOrUpdate<T>( T statusEffect, int stack ) where T : StatusEffect
	{
		var existing = Items.FirstOrDefault( x => x is T );

		if ( existing is not null )
		{
			existing.Stack = ApplyStackLimit( existing, existing.Stack, stack );
		}
		else
		{
			Log.EditorLog( "BEEEEE" );
			statusEffect.Owner = Owner;
			statusEffect.Stack = ClampStack( stack, statusEffect.Maximum );
			Items.Add( statusEffect );
		}
	}

	private void AddOrUpdateQueued( StatusEffect statusEffect, int stack )
	{
		var existing = _nextTurnItems.FirstOrDefault( x => x.StatusEffect.GetType() == statusEffect.GetType() );

		if ( existing is not null )
		{
			existing.Stack = ApplyStackLimit( existing.StatusEffect, existing.Stack, stack );
		}
		else
		{
			statusEffect.Owner = Owner;
			var queued = new QueuedStatusEffect
			{
				StatusEffect = statusEffect, Stack = ClampStack( stack, statusEffect.Maximum )
			};
			_nextTurnItems.Add( queued );
		}
	}

	public static int ClampStack( int stack, int? maximum )
	{
		return maximum.HasValue ? Math.Min( stack, maximum.Value ) : stack;
	}

	private static int ApplyStackLimit( StatusEffect effect, int current, int added )
	{
		if ( effect.Maximum is {} max )
		{
			return Math.Min( current + added, max );
		}

		return current + added;
	}

	protected override void OnStart()
	{
		if ( BattleManager.Instance.IsValid() )
		{
			BattleManager.Instance.OnTurnStart += OnTurnStart;
		}

		base.OnStart();
	}

	protected override void OnDestroy()
	{
		if ( BattleManager.Instance.IsValid() )
		{
			BattleManager.Instance.OnTurnStart -= OnTurnStart;
		}

		base.OnDestroy();
	}

	private void OnTurnStart()
	{
		Log.EditorLog( $"Turn start for: {Owner?.GameObject.Name}" );
		foreach ( var queued in _nextTurnItems )
		{
			Log.EditorLog( $"Queued: {queued}" );
			AddStatusEffect( queued.StatusEffect, queued.Stack );
		}

		_nextTurnItems.Clear();
	}

	// This is purposefully a class so we have mutability.
	public class QueuedStatusEffect
	{
		public required StatusEffect StatusEffect { get; set; }
		public int Stack { get; set; }

		public void Deconstruct( out StatusEffect effect, out int stack )
		{
			effect = StatusEffect;
			stack = Stack;
		}
	}
}
