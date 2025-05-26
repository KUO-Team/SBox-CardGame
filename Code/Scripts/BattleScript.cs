using Sandbox.Diagnostics;
using CardGame.Data;

namespace CardGame.Scripts;

public abstract class BattleScript
{
	protected static Logger Log { get; set; } = new( "BattleScript" );

	public virtual void OnLoad()
	{
		if ( BattleManager.Instance.IsValid() )
		{
			BattleManager.Instance.OnBattleStart += OnBattleStart;
			BattleManager.Instance.OnBattleEnd += OnBattleEnd;
			BattleManager.Instance.OnTurnStart += OnTurnStart;
			BattleManager.Instance.OnCombatStart += OnCombatStart;
			BattleManager.Instance.OnTurnEnd += OnTurnEnd;
		}
	}

	public virtual void OnUnload()
	{
		if ( BattleManager.Instance.IsValid() )
		{
			BattleManager.Instance.OnBattleStart -= OnBattleStart;
			BattleManager.Instance.OnBattleEnd -= OnBattleEnd;
			BattleManager.Instance.OnTurnStart -= OnTurnStart;
			BattleManager.Instance.OnCombatStart -= OnCombatStart;
			BattleManager.Instance.OnTurnEnd -= OnTurnEnd;
		}
	}

	public virtual void OnBattleStart( Battle battle )
	{

	}

	public virtual void OnBattleEnd( Battle battle )
	{

	}

	public virtual bool CanEndBattle( Faction winner )
	{
		return true;
	}

	public virtual void OnTurnStart()
	{

	}

	public virtual void OnCombatStart()
	{

	}

	public virtual void OnTurnEnd()
	{

	}
}
