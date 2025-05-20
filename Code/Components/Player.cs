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
		
		SetUnitData();

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

	private const int PlayerUnitId = 99;

	public void SetUnitData()
	{
		Unit = new UnitData();
		var unit = UnitDataList.GetById( PlayerUnitId );
		if ( unit is null )
		{
			return;
		}
		
		// Set the initial deck.
		var copy = unit.DeepCopy();
		Unit.Deck = copy.Deck;
		
		// Set initial health.
		Unit.Hp = copy.Hp;
		Unit.MaxHp = Unit.Hp;

		// Set initial level.
		Unit.Level = copy.Level;
		Unit.Xp = 0;
	}
}
