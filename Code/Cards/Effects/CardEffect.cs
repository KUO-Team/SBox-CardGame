using CardGame.Units;

namespace CardGame.Effects;

public abstract class CardEffect( Card card, RangedInt power ) : IEffect
{
	protected Card Card { get; set; } = card;
	
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

	public virtual void OnDraw( CardEffectDetail detail )
	{

	}
	
	public virtual void BeforePlay( CardEffectDetail detail )
	{
		
	}

	public virtual void OnPlay( CardEffectDetail detail )
	{
		
	}

	public virtual void OnDiscard( CardEffectDetail detail )
	{
		
	}
	
	public virtual void OnDiscardModeCardDiscard( CardEffectDetail detail, Card card )
	{

	}

	public virtual int DamageModifier( CardEffectDetail detail )
	{
		return 0;
	}

	public virtual bool CanPlay( CardEffectDetail detail )
	{
		return true;
	}

	public virtual bool CanAddToDeck( CardEffectDetail detail )
	{
		return true;
	}
	
	public struct CardEffectDetail
	{
		public BattleUnitComponent? Unit { get; set; }
		public BattleUnitComponent? Target { get; set; }
	}
}

public interface IEffect
{
	
}
