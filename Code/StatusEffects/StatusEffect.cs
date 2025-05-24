using CardGame.Data;
using CardGame.Units;

namespace CardGame.StatusEffects;

public abstract class StatusEffect : IOwnable
{
	public Data.StatusEffect Data { get; set; }

	public BattleUnit? Owner { get; set; }
	
	public int Stack { get; set; }

	protected StatusEffect( Data.StatusEffect data )
	{
		Data = data;

		if ( BattleManager.Instance.IsValid() )
		{
			BattleManager.Instance.OnBattleStart += OnBattleStart;
			BattleManager.Instance.OnBattleEnd += OnBattleEnd;
			BattleManager.Instance.OnCombatStart += OnCombatStart;
			BattleManager.Instance.OnTurnStart += OnTurnStart;
			BattleManager.Instance.OnTurnEnd += OnTurnEnd;
		}
	}

	public virtual StatusKey Keyword { get; set; }

	public virtual string Description()
	{
		return Data.Description;
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
		PowerUp,
		PowerDown,
		Protection,
		Fragile,
		Burn,
		Bleed,
		Stunned,
		Silenced,
		Enchanted,
		Haste,
		Cold,
		ManaUp,
		ManaDown
	}
}
