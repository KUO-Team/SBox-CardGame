namespace CardGame.Modifiers;

public sealed class CardModifiers
{
	private readonly List<ICardModifier> _modifiers = [];

	public void AddModifier( ICardModifier modifier )
	{
		_modifiers.Add( modifier );
	}
	
	public void RemoveModifier( ICardModifier modifier )
	{
		_modifiers.Remove( modifier );
	}

	public void TickDurations()
	{
		foreach ( var modifier in _modifiers )
		{
			modifier.Tick();
		}
		
		foreach ( var mod in _modifiers.ToList().Where( mod => mod.IsExpired ) )
		{
			_modifiers.Remove( mod );
		}
	}

	public int GetCostDelta()
	{
		return _modifiers.OfType<CostModifier>().Sum( costMod => costMod.Delta );
	}

	public int GetActionPowerDelta( Action action )
	{
		return _modifiers
			.OfType<ActionPowerModifier>()
			.Where( m => m.Targets( action ) )
			.Sum( m => m.Delta );
	}
	
	public int GetEffectPowerDelta()
	{
		return _modifiers
			.OfType<EffectPowerModifier>()
			.Sum( m => m.Delta );
	}
}

public interface ICardModifier
{
	bool IsExpired { get; }
	
	void Tick();
}
