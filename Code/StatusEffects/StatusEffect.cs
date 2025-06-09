using CardGame.Data;
using CardGame.Units;

namespace CardGame.StatusEffects;

public abstract class StatusEffect( Data.StatusEffect data ) : IOwnable
{
	public Data.StatusEffect Data { get; } = data;

	public BattleUnitComponent? Owner { get; set; }

	public int Stack { get; set; }

	public virtual StatusKey Keyword { get; set; }

	public virtual string Description()
	{
		return Data.Description;
	}

	public virtual void OnAdd()
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

	public virtual void OnUpdate()
	{

	}

	public virtual void OnAddOrUpdate()
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

	public virtual void OnTakeDamage( BattleUnitComponent dealer )
	{

	}

	public virtual void OnDealDamage( BattleUnitComponent target )
	{

	}

	public virtual void BeforePlayCard( Card card )
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

	public virtual int DamageModifier( int damage, Card? card )
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
		AttackPowerUp,
		AttackPowerDown,
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
