using CardGame.Data;
using CardGame.Units;

namespace CardGame.Passives;

public abstract class PassiveAbility : IResource, IOwnable
{
	public virtual Id Id { get; set; } = Id.Invalid;

	public virtual string Name { get; set; } = string.Empty;

	public virtual string Description { get; set; } = string.Empty;
	
	public BattleUnit? Owner { get; set; }
	
	protected PassiveAbility()
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

	public virtual void OnPlayCard( Card card )
	{

	}
	
	public virtual void AfterPlayCard( Card card )
	{

	}
	
	public virtual void OnDiscardCard( Card card )
	{

	}

	public virtual void OnTakeDamage( BattleUnit dealer )
	{

	}

	public virtual void OnDealDamage( BattleUnit target )
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
