using CardGame.Data;
using CardGame.Units;

namespace CardGame.Passives;

public abstract class PassiveAbility( Data.PassiveAbility data ) : IOwnable
{
	public Data.PassiveAbility Data { get; } = data;

	public BattleUnit? Owner { get; set; }

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
	
	public virtual void BeforePlayCard( Card card )
	{

	}

	public virtual void OnPlayCard( Card card )
	{

	}
	
	public virtual void OnDiscardCard( Card card )
	{

	}

	public virtual void OnTakeDamage( int damage, BattleUnit attacker )
	{

	}

	public virtual void OnDealDamage( int damage, BattleUnit target )
	{

	}

	public virtual int PowerModifier( Card card, Action action )
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
		
		Owner?.Passives?.Remove( this );
	}
}
