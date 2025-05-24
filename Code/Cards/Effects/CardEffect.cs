using CardGame.Units;

namespace CardGame.Effects;

public abstract class CardEffect( Card card )
{
	public Card Card { get; set; } = card;

	public RangedInt Power { get; set; } = 1;

	public virtual string Description { get; set; } = string.Empty;

	public virtual void OnDraw( CardEffectDetail detail )
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
		public BattleUnit? Unit { get; set; }
		public BattleUnit? Target { get; set; }
	}
}
