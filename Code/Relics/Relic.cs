using CardGame.Data;
using CardGame.Units;

namespace CardGame.Relics;

public abstract class Relic : IOwnable
{
	public Data.Relic Data { get; set; }
	
	public BattleUnit? Owner { get; set; }

	protected Relic( Data.Relic data )
	{
		Data = data;
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

	public virtual void OnCombatStart()
	{
		
	}

	public virtual void OnTurnStart()
	{
		
	}

	public virtual void OnTurnEnd()
	{
		
	}
	
	public virtual void OnPlayCard( Card card, BattleUnit unit )
	{

	}
	
	public virtual void AfterPlayCard( Card card, BattleUnit unit )
	{

	}

	public virtual void OnDiscardCard( Card card, BattleUnit unit )
	{
		
	}
	
	public virtual void OnTakeDamage( int damage, BattleUnit target, BattleUnit? attacker = null )
	{

	}
	
	public virtual void OnDealDamage( int damage, BattleUnit target, BattleUnit? attacker = null )
	{

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

		if ( RelicManager.Instance.IsValid() )
		{
			RelicManager.Instance.RemoveRelic( this );
		}
	}
}
