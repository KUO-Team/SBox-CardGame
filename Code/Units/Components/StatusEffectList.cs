﻿using System;
using CardGame.Data;
using StatusEffect=CardGame.StatusEffects.StatusEffect;

namespace CardGame.Units;

public class StatusEffectList : OwnableListComponent<StatusEffect>
{
	public IReadOnlyList<QueuedStatusEffect> NextTurnItems => _nextTurnItems.AsReadOnly();
	private readonly List<QueuedStatusEffect> _nextTurnItems = [];

	public void AddStatusEffect( StatusEffect statusEffect, int stack = 1 )
	{
		AddOrUpdate( statusEffect, stack );
	}
	
	public void AddStatusEffect( Data.StatusEffect data, int stack = 1 )
	{
		var statusEffect = StatusEffectDataList.GetById( data.Id );
		if ( statusEffect is null )
		{
			return;
		}
		
		var status = TypeLibrary.Create<StatusEffect>( statusEffect.Script, [data] );
		status.Stack = stack;
		AddOrUpdate( status, stack );
	}
	
	public void AddStatusEffectByKey( StatusEffect.StatusKey key, int stack = 1 )
	{
		var id = GetStatusEffectIdByKey( key );
		var statusEffect = StatusEffectDataList.GetById( id );
		if ( statusEffect is null )
		{
			return;
		}
		
		var status = TypeLibrary.Create<StatusEffect>( statusEffect.Script, [statusEffect] );
		status.Stack = stack;
		AddOrUpdate( status, stack );
	}
	
	public void AddStatusEffectNextTurn( StatusEffect statusEffect, int stack = 1 )
	{
		AddOrUpdateQueued( statusEffect, stack );
	}

	public void AddStatusEffectNextTurn( Data.StatusEffect data, int stack = 1 )
	{
		var statusEffect = StatusEffectDataList.GetById( data.Id );
		if ( statusEffect is null )
		{
			return;
		}
		
		var status = TypeLibrary.Create<StatusEffect>( statusEffect.Script, [data] );
		status.Stack = stack;
		AddOrUpdateQueued( status, stack );
	}
	
	public void AddStatusEffectByKeyNextTurn( StatusEffect.StatusKey key, int stack = 1 )
	{
		var id = GetStatusEffectIdByKey( key );
		var statusEffect = StatusEffectDataList.GetById( id );
		if ( statusEffect is null )
		{
			return;
		}
		
		var status = TypeLibrary.Create<StatusEffect>( statusEffect.Script, [statusEffect] );
		status.Stack = stack;
		AddOrUpdateQueued( status, stack );
	}

	public bool HasStatusEffect<T>() where T : StatusEffect
	{
		return Items.Any( x => x is T );
	}
	
	public bool HasQueuedStatusEffect<T>() where T : StatusEffect
	{
		return _nextTurnItems.Any( x => x.StatusEffect is T );
	}

	private void AddOrUpdate( StatusEffect statusEffect, int stack )
	{
		var existing = Items.FirstOrDefault( x => x.GetType() == statusEffect.GetType() );

		if ( existing is not null )
		{
			existing.Stack = ApplyStackLimit( existing, existing.Stack, stack );
			existing.OnUpdate();
			existing.OnAddOrUpdate();
		}
		else
		{
			statusEffect.Owner = Owner;
			statusEffect.Stack = ClampStack( statusEffect.Stack, statusEffect.Data.Maximum );
			Items.Add( statusEffect );
			statusEffect.OnAdd();
			statusEffect.OnAddOrUpdate();
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
				StatusEffect = statusEffect, Stack = ClampStack( stack, statusEffect.Data.Maximum )
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
		if ( effect.Data.Maximum is {} max )
		{
			return Math.Min( current + added, max );
		}

		return current + added;
	}

	public static StatusEffect? CreateStatusEffectByKey( StatusEffect.StatusKey key )
	{
		var id = GetStatusEffectIdByKey( key );
		if ( !id.IsValid )
		{
			return null;
		}
		
		var status = StatusEffectDataList.GetById( id );
		if ( status is null )
		{
			return null;
		}

		var statusEffect = TypeLibrary.Create<StatusEffect>( status.Script, [status] );
		return statusEffect;
	}

	internal static Id GetStatusEffectIdByKey( StatusEffect.StatusKey key )
	{
		return key switch
		{
			StatusEffect.StatusKey.AttackPowerUp => 1,
			StatusEffect.StatusKey.AttackPowerDown => 2,
			StatusEffect.StatusKey.Protection => 3,
			StatusEffect.StatusKey.Fragile => 4,
			StatusEffect.StatusKey.Burn => 5,
			StatusEffect.StatusKey.Bleed => 6,
			StatusEffect.StatusKey.Stunned => 7,
			StatusEffect.StatusKey.Silenced => 8,
			StatusEffect.StatusKey.Enchanted => 9,
			StatusEffect.StatusKey.Haste => 10,
			StatusEffect.StatusKey.Cold => 11,
			StatusEffect.StatusKey.ManaUp => 12,
			StatusEffect.StatusKey.ManaDown => 13,
			_ => throw new ArgumentOutOfRangeException( nameof( key ), key, null )
		};
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
		foreach ( var queued in _nextTurnItems )
		{
			AddStatusEffect( queued.StatusEffect, queued.Stack );
		}

		_nextTurnItems.Clear();
	}

	public class QueuedStatusEffect
	{
		public required StatusEffect StatusEffect { get; init; }
		public int Stack { get; set; }

		public void Deconstruct( out StatusEffect effect, out int stack )
		{
			effect = StatusEffect;
			stack = Stack;
		}
	}
}
