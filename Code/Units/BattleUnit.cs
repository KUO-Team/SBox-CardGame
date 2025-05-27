using System;
using System.Threading.Tasks;
using Sandbox.Diagnostics;

namespace CardGame.Units;

/// <summary>
/// Represents a character in battle.
/// </summary>
public partial class BattleUnit : BaseCharacter
{
	[Property]
	public string Name => GameObject.Name;

	[Property]
	public int Level { get; set; } = 1;

	[Property]
	public Faction Faction { get; set; } = Faction.Enemy;

	[Property, RequireComponent, Category( "Components" )]
	public HealthComponent? HealthComponent { get; set; }

	[Property, RequireComponent, Category( "Components" )]
	public HandComponent? HandComponent { get; set; }

	[Property, RequireComponent, Category( "Components" )]
	public PassiveAbilityList? Passives { get; set; }

	[Property, RequireComponent, Category( "Components" )]
	public StatusEffectList? StatusEffects { get; set; }

	[Property, RequireComponent, Category( "Components" )]
	public CardSlotList? Slots { get; set; }

	[Property, Category( "Energy" )]
	public int Energy { get; set; } = 3;

	[Property, Category( "Energy" )]
	public int MaxEnergy { get; set; }

	[Property, Category( "Mana" )]
	public int Mana { get; set; } = 3;

	[Property, Category( "Mana" )]
	public int MaxMana { get; set; } = 3;

	public static BattleUnit? ActiveUnit { get; set; }

	private static readonly Logger Log = new( "BattleUnit" );

	protected override void OnStart()
	{
		base.OnStart();
		GameObject.BreakFromPrefab();

		if ( ActiveUnit == null && Faction == Faction.Player )
		{
			ActiveUnit = this;
			Log.Info( $"Set active unit as: {this}" );

			var id = Data?.Id;
			if ( id is not null && id.Equals( 0 ) )
			{
				GameObject.Name = Connection.Local.DisplayName;
			}
		}

		if ( HealthComponent.IsValid() )
		{
			HealthComponent.OnDied += Die;
		}

		if ( BattleManager.Instance.IsValid() )
		{
			BattleManager.Instance.OnTurnEnd += OnTurnEnd;
		}
	}

	protected override void OnDestroy()
	{
		if ( ActiveUnit == this )
		{
			ActiveUnit = null;
		}

		if ( HealthComponent.IsValid() )
		{
			HealthComponent.OnDied -= Die;
		}

		if ( BattleManager.Instance.IsValid() )
		{
			BattleManager.Instance.OnTurnEnd -= OnTurnEnd;
		}

		base.OnDestroy();
	}

	public void RecoverEnergy( int amount )
	{
		Energy = Math.Min( MaxEnergy, Energy + amount );
	}

	public void SpendEnergy( int amount )
	{
		Energy = Math.Max( 0, Energy - amount );
	}

	public void RecoverMana( int amount )
	{
		Mana = Math.Min( MaxMana, Mana + amount );
	}

	public void SpendMana( int amount )
	{
		Mana = Math.Max( 0, Mana - amount );
	}

	private void OnTurnEnd()
	{
		if ( !HandComponent.IsValid() )
		{
			return;
		}

		foreach ( var card in HandComponent.Hand )
		{
			card.Modifiers.TickDurations();
		}
	}

	public async Task Die()
	{
		if ( SpriteComponent.IsValid() )
		{
			if ( HasAnimation( "die" ) )
			{
				SpriteComponent.PlayAnimation( "die" );
				await Task.DelaySeconds( 1 );
			}

			if ( HasAnimation( "dead" ) )
			{
				SpriteComponent.PlayAnimation( "dead" );
				await Task.DelaySeconds( 1 );
			}
			else
			{
				SpriteComponent.Sprite = null;
			}
		}

		if ( Slots.IsValid() )
		{
			foreach ( var slot in Slots )
			{
				slot.ClearTargetingArrows();
			}
		}

		var allUnitsDead = BattleManager.Units
			.Where( x => x.Faction == Faction )
			.All( x => x.HealthComponent.IsValid() && x.HealthComponent.IsDead );

		if ( !allUnitsDead )
		{
			return;
		}

		var oppositeFaction = Faction == Faction.Player ? Faction.Enemy : Faction.Player;
		BattleManager.Instance?.EndBattle( oppositeFaction );

		if ( Faction == Faction.Player )
		{
			GameManager.Instance?.EndRunInLoss();
		}
	}

	private bool HasAnimation( string name )
	{
		if ( !SpriteComponent.IsValid() )
		{
			return false;
		}

		if ( !SpriteComponent.Sprite.IsValid() )
		{
			return false;
		}

		var animation = SpriteComponent.Sprite.GetAnimation( name );
		return animation is not null;
	}
}
