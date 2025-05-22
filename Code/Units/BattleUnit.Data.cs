using System;
using CardGame.Data;
using CardGame.Passives;

namespace CardGame.Units;

public partial class BattleUnit
{
	public Unit? Data { get; private set; }

	public void SetData( Unit data, Faction faction = Faction.Enemy )
	{
		Data = data;
		GameObject.Name = data.Name;
		Level = data.Level;
		Faction = faction;

		if ( SpriteComponent.IsValid() )
		{
			if ( data.Sprite is not null )
			{
				if ( !data.Sprite.Resource.IsValid() )
				{
					return;
				}

				SpriteComponent.Sprite = data.Sprite.Resource;
			}
		}

		if ( HealthComponent.IsValid() )
		{
			HealthComponent.MaxHealth = data.Hp;
			HealthComponent.Health = HealthComponent.MaxHealth;
		}

		MaxEnergy = data.Ep;
		Energy = MaxEnergy;

		MaxMana = data.Mp;
		Mana = MaxMana;

		if ( HandComponent.IsValid() )
		{
			foreach ( var cardId in data.Deck )
			{
				var card = CardDataList.GetById( cardId );
				if ( card is null )
				{
					continue;
				}

				HandComponent.Deck.Add( card.DeepCopy() );
			}
		}

		if ( Slots.IsValid() )
		{
			Slots.AddCardSlot( data.Slots );
			foreach ( var slot in Slots )
			{
				slot.MinSpeed = data.Speed.Min;
				slot.MaxSpeed = data.Speed.Max;
			}
		}

		if ( Passives.IsValid() )
		{
			foreach ( var passiveId in data.Passives )
			{
				var passive = PassiveAbilityDataList.GetById( passiveId );
				if ( passive is not null )
				{
					Passives.AddPassiveAbility( passive );
				}
			}
		}

		if ( Faction == Faction.Enemy )
		{
			AddComponent<EnemyController>();
		}
	}

	public void ApplyLevelScaling( int level )
	{
		if ( Data is null )
		{
			Log.Warning( "Can't apply level scaling; no data found!" );
			return;
		}

		Level = level;

		var hp = GetScaledHp( Data.Hp );
		if ( HealthComponent.IsValid() )
		{
			HealthComponent.MaxHealth = hp;
			HealthComponent.Health = HealthComponent.MaxHealth;
		}
	}

	private int GetScaledHp( int hp )
	{
		return (int)MathF.Ceiling( hp * (1 + (Level - 1) * 0.1f) );
	}
}
