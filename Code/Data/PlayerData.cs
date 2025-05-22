using System;
using System.Text.Json.Serialization;
using Sandbox.Diagnostics;
using CardGame.Platform;
using CardGame.UI;

namespace CardGame.Data;

public class PlayerData
{
	public static PlayerData Data { get; private set; } = new();

	public const string FileName = "PlayerData.json";

	/// <summary>
	/// If this is the first time the player is playing the game.
	/// </summary>
	public bool FirstTime { get; set; } = true;

	public List<RunData> Runs { get; set; } = [];

	public List<Id> SeenCards { get; set; } = [];

	public List<Id> SeenRelics { get; set; } = [];

	public List<Id> Cards { get; set; } = [];

	public List<Id> CardPacks { get; set; } = [];

	private static readonly Logger Log = new( "PlayerData" );

	public void SeeCard( Id id )
	{
		if ( SeenCards.Contains( id ) )
		{
			return;
		}

		SeenCards.Add( id );
		if ( SeenCards.Count >= CardDataList.All.Count( x => x.IsAvailable ) )
		{
			Platform.Achievement.AllCards.Unlock();
		}
		Save();
	}

	public void SeeRelic( Id id )
	{
		if ( SeenRelics.Contains( id ) )
		{
			return;
		}

		SeenRelics.Add( id );
		if ( SeenRelics.Count >= RelicDataList.All.Count( x => x.IsAvailable ) )
		{
			Platform.Achievement.AllRelics.Unlock();
		}
		Save();
	}

	public void ValidateSeenCards()
	{
		var before = SeenCards.Count;

		SeenCards = SeenCards
			.Where( id => CardDataList.All.Any( card => card.Id.Equals( id ) && card.IsAvailable ) )
			.ToList();

		var after = SeenCards.Count;
		if ( before == after )
		{
			return;
		}

		Log.Warning( $"Removed {before - after} invalid seen cards." );
		Save();
	}

	public void ValidateSeenRelics()
	{
		var before = SeenRelics.Count;

		SeenRelics = SeenRelics
			.Where( id => RelicDataList.All.Any( relic => relic.Id.Equals( id ) && relic.IsAvailable ) )
			.ToList();

		var after = SeenRelics.Count;
		if ( before == after )
		{
			return;
		}

		Log.Warning( $"Removed {before - after} invalid seen relics." );
		Save();
	}

	public static void Clear()
	{
		Data.FirstTime = true;
		Data.Runs = [];
		Data.Cards = [];
		Data.CardPacks = [];
		Data.SeenCards = [];
		Data.SeenRelics = [];
		Save();
	}

	public static void Load()
	{
		Data = FileSystem.Data.ReadJson( FileName, new PlayerData() );
		Log.EditorLog( $"Loaded datafile from: {FileSystem.Data.GetFullPath( FileName )}" );
	}

	public static void Save()
	{
		foreach ( var run in Data.Runs )
		{
			run.Index = Data.Runs.IndexOf( run );
		}

		FileSystem.Data.WriteJson( FileName, Data );
		Log.EditorLog( $"Saved datafile to: {FileSystem.Data.GetFullPath( FileName )}" );
	}
}

public class RunData
{
	public int Index { get; set; }

	public int Seed { get; set; }

	public int MapNodeIndex { get; set; }

	public List<int> CompletedNodes { get; set; } = [];

	public int Floor { get; set; }

	public int Money { get; set; }

	public UnitData UnitData { get; set; } = new();

	public List<Id> Cards { get; set; } = [];

	public List<Id> CardPacks { get; set; } = [];

	public List<Id> Relics { get; set; } = [];

	public DateTime Date { get; set; } = DateTime.Now;
}

public class UnitData : IDeepCopyable<UnitData>
{
	public int Hp { get; set; }

	public int MaxHp { get; set; }

	public int Level { get; set; }

	public int Xp { get; set; }

	public List<Id> Deck { get; set; } = [];

	[Hide, JsonIgnore]
	public bool IsMaxHp => Hp >= MaxHp;

	public void Damage( int amount )
	{
		Hp = Math.Max( Hp - amount, 0 );

		if ( Hp > 0 )
		{
			return;
		}
		
		var panel = Game.ActiveScene.GetComponent<RunOverPanel>();
		if ( panel.IsValid() )
		{
			panel.EndRun();
		}
	}
	
	public void Heal( int amount )
	{
		Hp = Math.Min( Hp + amount, MaxHp );
	}

	public void HealToMax()
	{
		Hp = MaxHp;
	}

	public UnitData DeepCopy()
	{
		return new UnitData
		{
			Hp = Hp, MaxHp = MaxHp, Deck = [..Deck]
		};
	}
}
