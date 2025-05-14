using CardGame.Data;
using CardGame.Units;

namespace CardGame.StatusEffects;

public abstract class StatusEffect : IResource, IOwnable
{
	public virtual Id Id { get; set; } = Id.Invalid;

	[ImageAssetPath]
	public virtual string Icon { get; set; } = string.Empty;

	public virtual string Name { get; set; } = string.Empty;

	public virtual string Description { get; set; } = string.Empty;

	public virtual bool IsNegative { get; set; }

	public virtual bool IsHiddenStack { get; set; }

	public virtual Color Color { get; set; } = Color.FromBytes( 255, 215, 0 );

	public virtual int? Maximum { get; set; }
	
	public virtual StatusKey Keyword { get; set; }
	
	public BattleUnit? Owner { get; set; }

	public int Stack { get; set; }

	protected StatusEffect()
	{
		if ( BattleManager.Instance.IsValid() )
		{
			BattleManager.Instance.OnBattleStart += OnBattleStart;
			BattleManager.Instance.OnBattleEnd += OnBattleEnd;
			BattleManager.Instance.OnCombatStart += OnCombatStart;
			BattleManager.Instance.OnTurnStart += OnTurnStart;
			BattleManager.Instance.OnTurnEnd += OnTurnEnd;
		}
	}

	public virtual void OnAdd()
	{
		
	}

	public virtual void OnBattleStart( Battle battle )
	{
		
	}

	public virtual void OnBattleEnd( Battle battle )
	{
		
	}

	public virtual void OnTurnStart()
	{
		
	}

	public virtual void OnTurnEnd()
	{
		
	}

	public virtual void OnCombatStart()
	{
		
	}

	public virtual void OnTakeDamage( BattleUnit dealer )
	{

	}

	public virtual void OnDealDamage( BattleUnit target )
	{

	}

	public virtual void OnPlayCard( Card card )
	{

	}
	
	public virtual void AfterPlayCard( Card card )
	{

	}

	public virtual void OnDiscardCard( Card card )
	{

	}
	
	public virtual int PowerModifier( Card card, Action action )
	{
		return 0;
	}

	public virtual int DamageModifier( Card card, int damage )
	{
		return 0;
	}

	public virtual void Destroy()
	{
		if ( BattleManager.Instance.IsValid() )
		{
			BattleManager.Instance.OnBattleStart -= OnBattleStart;
			BattleManager.Instance.OnBattleEnd -= OnBattleEnd;
			BattleManager.Instance.OnCombatStart -= OnCombatStart;
			BattleManager.Instance.OnTurnStart -= OnTurnStart;
			BattleManager.Instance.OnTurnEnd -= OnTurnEnd;
		}
		
		Owner?.StatusEffects?.Remove( this );
	}
	
	public enum StatusKey
	{
		Bleed,
		Burn,
		Enchanted,
		Fragile,
		Protection,
		PowerDown,
		PowerUp,
		Immobilized,
		Silenced
	}
}
