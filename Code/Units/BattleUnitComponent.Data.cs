using System;
using CardGame.Data;
using CardGame.Passives;

namespace CardGame.Units;

public partial class BattleUnitComponent
{
	public Unit? Data { get; private set; }

	public void SetData( Unit data, Faction faction = Faction.Enemy )
	{
		Data = data;
		GameObject.Name = data.Name;
		Description = data.Description;
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

		MaxMana = data.Mp;
		Mana = MaxMana;

		if ( Slots.IsValid() )
		{
			Slots.AddCardSlot( data.Slots );
			foreach ( var slot in Slots )
			{
				slot.MinSpeed = data.Speed.Min;
				slot.MaxSpeed = data.Speed.Max;
			}
		}

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

		if ( Passives.IsValid() )
		{
			foreach ( var passiveId in data.Passives )
			{
				var passive = PassiveAbilityDataList.GetById( passiveId );
				if ( passive is null )
				{
					continue;
				}
				
				Passives.AddPassiveAbility( passive );
			}
		}

		if ( Faction == Faction.Enemy )
		{
			AddComponent<EnemyController>();
		}
	}

	public void ApplyLevelScaling()
	{
		if ( Data is null )
		{
			Log.Warning( "Can't apply level scaling; no data found!" );
			return;
		}

		var hp = GetScaledHp( Data.Hp );
		if ( HealthComponent.IsValid() )
		{
			HealthComponent.MaxHealth = hp;
			HealthComponent.Health = HealthComponent.MaxHealth;
		}
	}

	private int GetScaledHp( int hp )
	{
		if ( !HealthComponent.IsValid() )
		{
			return 100;
		}
		
		if ( !LevelComponent.IsValid() )
		{
			return HealthComponent.Health;
		}
		
		return (int)MathF.Ceiling( hp * (1 + (LevelComponent.Level - 1) * 0.1f) );
	}
}
