using CardGame.Data;
using CardGame.Units;

namespace CardGame;

public sealed partial class BattleManager
{
	[Property, Category( "Prefabs" )]
	public GameObject? UnitPrefab { get; set; }

	public Id? PlayerUnit { get; set; }

	public static List<BattleUnit> Units => Game.ActiveScene.GetAllComponents<BattleUnit>().ToList();

	public static List<BattleUnit> AliveUnits => Units.Where( x => x.HealthComponent.IsValid() && !x.HealthComponent.IsDead ).ToList();

	public BattleUnit? SpawnUnitFromData( Unit data, Faction faction, SpriteFlags spriteFlags = SpriteFlags.None, Transform? transform = null )
	{
		if ( !UnitPrefab.IsValid() )
		{
			Log.Warning( $"No unit prefab set; unable to spawn unit!" );
			return null;
		}

		var gameObject = !data.Prefab.IsValid() ? UnitPrefab.Clone() : data.Prefab.Clone();
		if ( gameObject.Components.TryGet( out BattleUnit unit ) is not true )
		{
			Log.Warning( $"No unit component found; unable to set data!" );
			return null;
		}

		var spawnPoint = GetRandomAvailableSpawnPoint( faction );
		if ( !spawnPoint.IsValid() )
		{
			return null;
		}

		spawnPoint.Place( gameObject );
		if ( transform.HasValue )
		{
			unit.WorldTransform = transform.Value;
		}
		
		unit.SetData( data, faction );

		if ( faction == Faction.Player && spriteFlags == SpriteFlags.None )
		{
			if ( unit.SpriteComponent.IsValid() )
			{
				unit.SpriteComponent.SpriteFlags = SpriteFlags.HorizontalFlip;
			}
		}

		Log.Info( $"Spawned unit {data.Name}" );
		return unit;
	}

	public UnitSpawnPoint? GetRandomAvailableSpawnPoint( Faction? faction = null )
	{
		var spawnPoints = Scene.GetAllComponents<UnitSpawnPoint>();

		if ( faction.HasValue )
		{
			spawnPoints = spawnPoints.Where( x => x.Faction == faction.Value );
		}

		var availableSpawnPoints = spawnPoints
			.Where( x => !x.IsOccupied )
			.OrderByDescending( x => x.Order )
			.ToList();

		return availableSpawnPoints.FirstOrDefault();
	}

	public BattleUnit? SpawnPlayerUnit()
	{
		if ( PlayerUnit is null )
		{
			Log.Warning( "Unable to spawn player unit; no player unit set!" );
			return null;
		}

		var playerUnit = UnitDataList.GetById( PlayerUnit );
		if ( playerUnit is null )
		{
			Log.Warning( $"Unable to spawn player unit; no unit data found with ID: {PlayerUnit}" );
			return null;
		}

		var unit = SpawnUnitFromData( playerUnit, Faction.Player );
		if ( unit is null )
		{
			return null;
		}

		unit.HandComponent?.Deck.Clear();
		unit.HandComponent?.Hand.Clear();

		unit.GameObject.Name = Connection.Local.DisplayName;

		if ( !Player.Local.IsValid() || Player.Local.Unit is null )
		{
			return unit;
		}
		
		foreach ( var cardId in Player.Local.Unit.Deck )
		{
			var card = CardDataList.GetById( cardId );
			if ( card is not null )
			{
				unit.HandComponent?.Deck.Add( card );
			}
		}

		if ( unit.HealthComponent.IsValid() )
		{
			unit.HealthComponent.Health = Player.Local.Unit.Hp;
			unit.HealthComponent.MaxHealth = Player.Local.Unit.MaxHp;
			unit.Mana = Player.Local.Unit.Mp;
			unit.MaxMana = Player.Local.Unit.MaxMp;
		}

		Player.Local.Units.Add( unit );
		return unit;
	}

	public static List<BattleUnit> GetUnits( Faction faction )
	{
		return Units.Where( x => x.Faction == faction ).ToList();
	}

	public static List<BattleUnit> GetAliveUnits( Faction faction )
	{
		return AliveUnits.Where( x => x.Faction == faction ).ToList();
	}

	public static BattleUnit? GetRandomUnit( Faction faction )
	{
		return Game.Random.FromList( GetUnits( faction )! );
	}

	public static BattleUnit? GetRandomAliveUnit( Faction faction )
	{
		return Game.Random.FromList( GetAliveUnits( faction )! );
	}
}
