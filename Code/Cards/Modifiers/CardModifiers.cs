namespace CardGame.Modifiers;

public sealed class CardModifiers
{
	private readonly List<ICardModifier> _modifiers = [];

	public void AddModifier( ICardModifier modifier )
	{
		_modifiers.Add( modifier );
	}

	public void TickDurations()
	{
		foreach ( var mod in _modifiers.ToList().Where( mod => mod.IsExpired ) )
		{
			_modifiers.Remove( mod );
		}
	}

	public Card.CardCost GetCostDelta()
	{
		var total = new Card.CardCost();
		foreach ( var costMod in _modifiers.OfType<CostModifier>() )
		{
			total.Ep += costMod.EpDelta;
			total.Mp += costMod.MpDelta;
		}
		
		return total;
	}

	public int GetPowerDelta( Action action )
	{
		return _modifiers
			.OfType<PowerModifier>()
			.Where( m => m.Targets( action ) )
			.Sum( m => m.Delta );
	}
}

public interface ICardModifier
{
	bool IsExpired { get; }
}
