﻿using System;
using System.Threading.Tasks;
using Sandbox.Audio;
using Sandbox.Diagnostics;
using CardGame.Data;
using CardGame.UI;
using CardGame.Units;

namespace CardGame;

public sealed partial class BattleManager : Singleton<BattleManager>
{
	[Property, RequireComponent, Category( "Components" )]
	public BattleHud? Hud { get; set; }

	[Property, ReadOnly]
	public bool IsTutorial => Battle is not null && Battle.Id.Equals( 1 );

	[Property, ReadOnly]
	public bool ShowEndScreen { get; set; } = true;

	[Property]
	public TurnState State { get; set; } = TurnState.Start;

	[Property]
	public int Turn { get; set; }

	[Property]
	public bool CanEndTurn { get; set; } = true;

	public Battle? Battle { get; set; }

	public SoundHandle? Bgm { get; set; }

	public event Action<Battle>? OnBattleStart;
	public event Action<Battle>? OnBattleEnd;

	public event System.Action? OnTurnStart;
	public event System.Action? OnCombatStart;
	public event System.Action? OnTurnEnd;

	private static SceneManager? SceneManager => SceneManager.Instance;

	private static readonly Logger Log = new( "BattleManager" );

	protected override void OnStart()
	{
		GameObject.Flags = GameObjectFlags.DontDestroyOnLoad;
		base.OnStart();
	}

	protected override async void OnUpdate()
	{
		try
		{
			if ( !CanEndTurn )
			{
				return;
			}

			if ( Input.Pressed( "SkipTurn" ) )
			{
				await StartCombat();
			}

			base.OnUpdate();
		}
		catch ( Exception e )
		{
			Log.Warning( e );
		}
	}

	public async Task StartBattle( Battle battle )
	{
		if ( SceneManager.IsValid() )
		{
			SceneManager.LoadScene( SceneManager.Scenes.Battle );
		}

		Battle = battle;
		State = TurnState.Start;
		ShowEndScreen = true;
		InputComponent.SelectedSlot = null;

		Bgm = Sound.Play( Battle.Bgm );
		if ( Bgm.IsValid() )
		{
			Bgm.TargetMixer = Mixer.FindMixerByName( "Music" );
		}

		if ( !string.IsNullOrEmpty( Battle.Script ) )
		{
			LoadBattleScript( Battle.Script );
		}

		if ( Hud.IsValid() )
		{
			Hud.OnBattleStart();
			if ( Hud.TurnStartPanel.IsValid() )
			{
				Hud.TurnStartPanel.DontShow = false;
			}
		}

		foreach ( var unit in battle.EnemyUnits )
		{
			var unitData = UnitDataList.GetById( unit.Id );
			if ( unitData is not null )
			{
				BattleUnitComponent? battleUnit = null;
				var mapManager = MapManager.Instance;
				if ( mapManager.IsValid() )
				{
					var enemyLevel = unit.UseFloorLevel ? mapManager.EnemyLevel : unit.BaseLevel;
					battleUnit = SpawnUnitFromData( unitData, Faction.Enemy, level: enemyLevel );
				}
				else
				{
					battleUnit = SpawnUnitFromData( unitData, Faction.Enemy, level: unit.BaseLevel );
				}
				
				if ( !battleUnit.IsValid() )
				{
					continue;
				}

				if ( battleUnit.LevelComponent.IsValid() )
				{
					battleUnit.LevelComponent.Level = unit.BaseLevel;
					if ( unit.UseLevelScaling )
					{
						battleUnit.ApplyLevelScaling();
					}
				}
			}
			else
			{
				Log.Warning( $"Unable to spawn enemy unit; no unit data found with ID: {unit.Id}" );
			}
		}

		if ( !battle.PlayerUnits.Any() )
		{
			SpawnPlayerUnit();
		}
		else
		{
			foreach ( var unitId in battle.PlayerUnits )
			{
				var unitData = UnitDataList.GetById( unitId );
				if ( unitData is not null )
				{
					SpawnUnitFromData( unitData, Faction.Player );
				}
			}
		}

		foreach ( var unit in Units )
		{
			unit.HandComponent?.DrawX( 3 );
		}

		Turn = 0;
		OnBattleStart?.Invoke( battle );
		Log.Info( $"Started Battle {battle.Name}" );

		await Task.DelaySeconds( 2 );
		StartTurn();
	}

	public void EndBattle( Faction winner, bool force = false )
	{
		if ( Battle is null )
		{
			Log.Warning( "Unable to end the battle; no active battle!" );
			return;
		}

		if ( !force )
		{
			if ( BattleScript is not null && !BattleScript.CanEndBattle( winner ) )
			{
				return;
			}
		}

		InputComponent.SelectedSlot = null;
		
		var player = Player.Local;
		if ( player.IsValid() )
		{
			var playerUnit = player.Unit;
			if ( playerUnit is not null )
			{
				foreach ( var unit in Units )
				{
					if ( unit.Faction != Faction.Player )
					{
						continue;
					}
			
					if ( unit.HealthComponent.IsValid() && !unit.HealthComponent.IsDead )
					{
						playerUnit.Hp = unit.HealthComponent.Health;
					}
				}
			}
		}

		Log.Info( $"Battle over. Winner: {winner}" );

		if ( Hud.IsValid() && Hud.TurnStartPanel.IsValid() )
		{
			Hud.TurnStartPanel.DontShow = true;
		}

		Bgm?.Stop( 1f );
		Hud?.OnBattleEnd( winner, Battle );
		OnBattleEnd?.Invoke( Battle );
		UnloadBattleScript();
	}

	internal void ForceEndBattle()
	{
		Bgm?.Stop( 1f );
		InputComponent.SelectedSlot = null;
		Battle = null;
		UnloadBattleScript();
	}

	public async Task StartCombat()
	{
		CanEndTurn = false;
		OnCombatStart?.Invoke();
		State = TurnState.Combat;
		InputComponent.SelectedSlot = null;

		var occupiedSlots = Scene.GetAllComponents<CardSlot>()
			.Where( slot => slot.IsAssigned )
			.ToList();

		foreach ( var slot in occupiedSlots.OrderByDescending( x => x.Speed ) )
		{
			if ( !slot.Owner.IsValid() || !slot.Target.IsValid() )
			{
				continue;
			}

			var target = slot.Target.Owner;
			if ( !target.IsValid() )
			{
				continue;
			}

			if ( target.HealthComponent.IsValid() && target.HealthComponent.IsDead )
			{
				continue;
			}

			await slot.PlayAsync( slot.Target );
		}

		State = TurnState.End;
		EndTurn();
	}

	internal void StartTurn()
	{
		InputComponent.SelectedSlot = null;
		CanEndTurn = true;
		State = TurnState.Start;
		Turn++;

		OnTurnStart?.Invoke();
		Hud?.OnTurnStart();
	}

	public void EndTurn()
	{
		OnTurnEnd?.Invoke();

		foreach ( var unit in Units )
		{
			unit.HandComponent?.Draw();
			unit.RecoverMana( 1 );
		}

		StartTurn();
	}
}

public enum TurnState
{
	Start,
	Combat,
	End
}

public enum Faction
{
	Enemy,
	Player
}

public static class FactionExtensions
{
	public static Faction GetOpposite( this Faction faction )
	{
		return faction == Faction.Player ? Faction.Enemy : Faction.Player;
	}
}
