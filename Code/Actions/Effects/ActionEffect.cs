using CardGame.Units;

namespace CardGame.Effects;

public abstract class ActionEffect( Card card, Action action, RangedInt power ) : IEffect
{
	protected Card Card { get; set; } = card;

	protected Action Action { get; set; } = action;
	
	protected RangedInt PowerRange { get; set; } = power;

	protected int EffectivePower
	{
		get
		{
			var defaultPower = PowerRange.Value;
			var delta = Card.Modifiers.GetEffectPowerDelta();
			var modifiedPower = defaultPower + delta;
			
			Log.EditorLog( $"Power: {modifiedPower} | Base Roll: {defaultPower} | Delta: {delta}" );
			return modifiedPower;
		}
	}

	public virtual string Description { get; set; } = string.Empty;

	public virtual void OnHit( ActionEffectDetail detail )
	{
		
	}
	
	public struct ActionEffectDetail
	{
		public BattleUnitComponent Unit { get; set; }
		public BattleUnitComponent? Target { get; set; }
	}
}
