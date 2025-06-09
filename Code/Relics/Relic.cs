using CardGame.Data;
using CardGame.Units;

namespace CardGame.Relics;

public abstract class Relic( Data.Relic data ) : IOwnable
{
	public Data.Relic Data { get; } = data;

	public BattleUnitComponent? Owner { get; set; }

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
	
	public virtual void BeforePlayCard( Card card, BattleUnitComponent unitComponent )
	{
		
	}
	
	public virtual void OnPlayCard( Card card, BattleUnitComponent unitComponent )
	{
		
	}
	
	public virtual void OnDiscardCard( Card card, BattleUnitComponent unitComponent )
	{
		
	}
	
	public virtual void OnTakeDamage( int damage, BattleUnitComponent target, BattleUnitComponent? attacker = null )
	{
		
	}
	
	public virtual void OnDealDamage( int damage, BattleUnitComponent target, BattleUnitComponent? attacker = null )
	{
		
	}

	public virtual void OnActivate()
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
			RelicManager.Instance.ShownRelics.Remove( this );
			RelicManager.Instance.Relics.Remove( this );
		}
	}
}
