﻿using CardGame.Data;
using CardGame.Units;

namespace CardGame;

public class Player : Component
{
	[Property]
	public static Player? Local { get; private set; }

	[Property]
	public UnitData? Unit { get; private set; }

	[Property, ReadOnly]
	public PlayerClass? Class { get; private set; }

	[Property]
	public List<BattleUnitComponent> Units { get; set; } = [];

	[Property]
	public List<CardPack> CardPacks { get; set; } = [];

	[Property]
	public List<Card> Cards { get; set; } = [];

	[Property]
	public int Money { get; set; }

	protected override void OnStart()
	{
		if ( !IsProxy )
		{
			Local = this;
		}

		GameObject.BreakFromPrefab();
		GameObject.Flags = GameObjectFlags.DontDestroyOnLoad;
		
		var firstClass = PlayerClassDataList.All[0];
		SetClass( firstClass );

		base.OnStart();
	}

	private Id? _playerUnitId;

	public void SetUnitData( Id id )
	{
		_playerUnitId = id;
		if ( BattleManager.Instance.IsValid() )
		{
			BattleManager.Instance.PlayerUnit = _playerUnitId;
		}

		var source = UnitDataList.GetById( id );
		if ( source is null )
		{
			Unit = null;
			return;
		}

		Unit = new UnitData();
		ApplyUnitTemplate( Unit, source );
	}

	public void ResetUnitData()
	{
		if ( _playerUnitId is null )
		{
			Log.Error( $"No player unit id found!" );
			return;
		}

		if ( Unit is null )
		{
			Log.Error( "Cannot reset unit data before setting it." );
			return;
		}

		var source = UnitDataList.GetById( _playerUnitId );
		if ( source is null )
		{
			Log.Error( $"Unit data with ID {_playerUnitId} not found." );
			return;
		}

		ApplyUnitTemplate( Unit, source );
	}

	private static void ApplyUnitTemplate( UnitData target, Unit template )
	{
		var copy = template.DeepCopy();
		target.Deck = copy.Deck;
		target.Hp = copy.Hp;
		target.MaxHp = copy.Hp;
		target.Mp = copy.Mp;
		target.MaxMp = copy.Mp;
		target.Xp = 0;
	}

	public void SetClass( PlayerClass playerClass )
	{
		Class = playerClass;
		SetUnitData( playerClass.Unit );
	}

	public void SetClassById( Id classId )
	{
		var data = PlayerClassDataList.GetById( classId );
		if ( data is null )
		{
			return;
		}

		SetClass( data );
	}
}
