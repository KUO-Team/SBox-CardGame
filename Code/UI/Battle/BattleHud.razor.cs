using System;
using System.Threading.Tasks;
using CardGame.Data;
using Sandbox.UI;

namespace CardGame.UI;

public partial class BattleHud
{
	[Property, RequireComponent]
	public BattleManager? BattleManager { get; set; }

	public BattleStartPanel? BattleStartPanel { get; set; }
	public BattleEndPanel? BattleEndPanel { get; set; }
	public TurnStartPanel? TurnStartPanel { get; set; }

	protected override void OnStart()
	{
		Mouse.Visibility = MouseVisibility.Visible;
		base.OnStart();
	}

	public async Task EndTurn()
	{
		if ( !BattleManager.IsValid() )
		{
			return;
		}
		
		switch ( BattleManager.State )
		{
			case TurnState.Start:
				await BattleManager.StartCombat();
				break;
			case TurnState.End:
				BattleManager.StartTurn();
				break;
			case TurnState.Combat:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void OnTurnStart()
	{
		if ( TurnStartPanel.IsValid() )
		{
			TurnStartPanel.OnTurnStart();
		}
	}

	public void OnBattleStart()
	{
		if ( BattleStartPanel.IsValid() )
		{
			BattleStartPanel.OnBattleStart();
		}
	}

	public void OnBattleEnd( Faction winner, Battle battle )
	{
		if ( !BattleEndPanel.IsValid() )
		{
			return;
		}

		if ( BattleManager.IsValid() && !BattleManager.ShowEndScreen )
		{
			return;
		}

		var isWin = winner == Faction.Player;
		BattleEndPanel.IsWin = isWin;

		if ( isWin )
		{
			BattleEndPanel.OnWin( battle.Rewards );
		}
	}

	private Panel? _cardShowcase;

	public void ShowCard( Card card )
	{
		if ( !_cardShowcase.IsValid() )
		{
			return;
		}

		var panel = new CardPanel
		{
			Card = card
		};
		_cardShowcase.AddChild( panel );
	}

	public void RemoveCard()
	{
		if ( !_cardShowcase.IsValid() )
		{
			return;
		}

		_cardShowcase.DeleteChildren();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( BattleManager?.Turn, BattleManager?.CanEndTurn, RelicManager.Instance?.Relics.Count, BattleManager.AliveUnits.Count );
	}
}
