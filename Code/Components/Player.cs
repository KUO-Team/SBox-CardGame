using CardGame.Data;
using CardGame.Units;

namespace CardGame;

public class Player : Component
{
	[Property]
	public static Player? Local { get; private set; }

	[Property]
	public UnitData? Unit { get; private set; }

	[Property]
	public List<BattleUnit> Units { get; set; } = [];

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

		SetUnitData( _playerUnitId );

		GameObject.BreakFromPrefab();
		GameObject.Flags = GameObjectFlags.DontDestroyOnLoad;

		foreach ( var packId in PlayerData.Data.CardPacks )
		{
			var cardPack = CardPackDataList.GetById( packId );
			if ( cardPack is not null )
			{
				CardPacks.Add( cardPack );
			}
		}

		foreach ( var cardId in PlayerData.Data.Cards )
		{
			var card = CardDataList.GetById( cardId );
			if ( card is not null )
			{
				Cards.Add( card );
			}
		}

		base.OnStart();
	}

	private Id _playerUnitId = 99;

	public void SetUnitData( Id id )
	{
		_playerUnitId = id;

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
		target.Xp = 0;
	}
}
