namespace CardGame.Modifiers;

public class CostModifier : ICardModifier
{
	public int Delta { get; }

	public bool IsExpired => _remainingTurns <= 0;
	
	private int _remainingTurns;

	// ReSharper disable once ConvertToPrimaryConstructor
	public CostModifier( int delta, int duration )
	{
		Delta = delta;
		_remainingTurns = duration;
	}

	public void Tick() 
	{
		_remainingTurns--;
	}
}
