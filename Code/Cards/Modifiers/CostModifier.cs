namespace CardGame.Modifiers;

public class CostModifier : ICardModifier
{
	public int EpDelta { get; }
	
	public int MpDelta { get; }

	public bool IsExpired => _remainingTurns <= 0;
	
	private int _remainingTurns;

	// ReSharper disable once ConvertToPrimaryConstructor
	public CostModifier( int epDelta, int mpDelta, int duration )
	{
		EpDelta = epDelta;
		MpDelta = mpDelta;
		_remainingTurns = duration;
	}

	public void Tick() 
	{
		_remainingTurns--;
	}
}
