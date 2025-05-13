using System;

namespace CardGame.Modifiers;

public class PowerModifier : ICardModifier
{
	public int Delta { get; }

	public bool IsExpired => _remainingTurns <= 0;
		
	private readonly Func<Action, bool> _filter;

	private int _remainingTurns;

	// ReSharper disable once ConvertToPrimaryConstructor
	public PowerModifier( int delta, Func<Action, bool> filter, int duration )
	{
		Delta = delta;
		_filter = filter;
		_remainingTurns = duration;
	}

	public bool Targets( Action action ) 
	{
		return _filter( action );
	}

	public void Tick()
	{
		_remainingTurns--;
	}
}
