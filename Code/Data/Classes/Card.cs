using System;
using System.Text.Json.Serialization;
using Sandbox.Diagnostics;
using CardGame.Data;
using CardGame.Units;
using CardGame.Effects;
using CardGame.Modifiers;

namespace CardGame;

public class Card : IResource, IDeepCopyable<Card>
{
	[InlineEditor]
	public Id Id { get; set; } = Id.Invalid;

	public string Name { get; set; } = string.Empty;

	public int Cost { get; init; }

	public bool IsInstant { get; set; }

	public int Targets { get; set; } = 1;

	public CardTargets Targeting { get; set; } = CardTargets.Enemy;

	public CardType Type { get; set; } = CardType.Attack;

	public CardRarity Rarity { get; set; } = CardRarity.Common;

	public CardAvailabilities Availabilities { get; set; } = CardAvailabilities.None;

	public TagSet Keywords { get; set; } = new();

	[Title( "Card Effect" )]
	[InlineEditor]
	public CardEffect Effect { get; set; } = new();

	[Hide, JsonIgnore]
	public Effects.CardEffect? ActiveEffect { get; set; }

	[InlineEditor, WideMode]
	public List<Action> Actions { get; set; } = [];

	[Hide, JsonIgnore]
	public bool IsAvailable
	{
		get
		{
			if ( Availabilities.HasFlag( CardAvailabilities.Starter ) )
			{
				return true;
			}

			if ( Availabilities.HasFlag( CardAvailabilities.Shop ) )
			{
				return true;
			}

			if ( Availabilities.HasFlag( CardAvailabilities.Event ) )
			{
				return true;
			}

			return false;
		}
	}

	[Hide, JsonIgnore]
	public CardModifiers Modifiers { get; } = new();

	[Hide, JsonIgnore]
	public int EffectiveCost
	{
		get
		{
			var delta = Modifiers.GetCostDelta();
			return Math.Max( 0, Cost + delta );
		}
	}

	private static readonly Logger Log = new( "Card" );
	
	public void InitEffect()
	{
		if ( string.IsNullOrWhiteSpace( Effect.Script ) )
		{
			return;
		}

		ActiveEffect = TypeLibrary.Create<Effects.CardEffect>( Effect.Script, [this, Effect.Power] );
	}

	/// <summary>
	/// Plays the card on the specified target using the given slot.
	/// </summary>
	public void Play( CardSlot slot, CardSlot target )
	{
		var owner = slot.Owner;
		if ( !owner.IsValid() || !owner.HealthComponent.IsValid() || owner.HealthComponent.IsDead )
		{
			return;
		}
		
		var targetUnit = target.Owner;
		if ( !targetUnit.IsValid() || !targetUnit.HealthComponent.IsValid() || targetUnit.HealthComponent.IsDead )
		{
			return;
		}

		var selectedTargets = SelectTargets( targetUnit, owner );
		foreach ( var selected in selectedTargets )
		{
			TriggerBeforePlayEffects( owner, selected );
			PlayOnTarget( owner, targetUnit, target.AssignedCard );
			TriggerOnPlayEffects( owner, selected );
		}

		slot.AssignedCard = null;
		if ( Type != CardType.Item )
		{
			owner.HandComponent?.Deck.Add( this );
		}
		else if ( Player.Local?.Unit is {} localUnit )
		{
			localUnit.Deck.Remove( Id );
		}
	}

	private void PlayOnTarget( BattleUnitComponent owner, BattleUnitComponent target, Card? opposingCard )
	{
		foreach ( var action in Actions )
		{
			var power = action.GetPowerRoll( owner, this );
			
			switch ( action.Type )
			{
				case Action.ActionType.Attack:
					var damage = power;
					if ( ActiveEffect is not null )
					{
						damage += ActiveEffect.DamageModifier( CreateCardDetail( owner, target ) );
					}
					
					Log.EditorLog( $"{Name} | Power: {power} | Damage: {damage}" );

					// ReSharper disable once UseNullPropagation
					if ( opposingCard is not null )
					{
						var defenseAction = opposingCard.Actions.FirstOrDefault( a => a.Type == Action.ActionType.Defense );
						if ( defenseAction is not null )
						{
							var defensePower = defenseAction.GetPowerRoll( target, opposingCard );
							Log.EditorLog( $"{opposingCard.Name} | Defense Power: {defensePower}" );
							damage -= defensePower;
						}
					}
					
					target.HealthComponent?.TakeDamage( damage, owner );
					if ( damage > 0 && action.ActiveEffect is not null )
					{
						action.ActiveEffect.OnHit( CreateActionDetail( owner, target ) );
					}
					break;
				case Action.ActionType.Defense:
					break;
				default:
					throw new ArgumentOutOfRangeException( action.Type.ToString() );
			}

			TriggerDamageEvents( owner, target );
		}
	}

	private List<BattleUnitComponent> SelectTargets( BattleUnitComponent mainTarget, BattleUnitComponent owner )
	{
		var allValid = GetValidTargets( owner, Targeting ).Where( u => u != mainTarget ).ToList();
		var targets = new List<BattleUnitComponent>
		{
			mainTarget
		};

		if ( Targets > 1 )
		{
			targets.AddRange( allValid.OrderBy( _ => Game.Random.Next() ).Take( Targets - 1 ) );
		}

		return targets;
	}

	public static List<BattleUnitComponent> GetValidTargets( BattleUnitComponent user, CardTargets targeting )
	{
		var targets = new List<BattleUnitComponent>();

		if ( targeting.HasFlag( CardTargets.Self ) )
		{
			targets.Add( user );
		}

		if ( targeting.HasFlag( CardTargets.Enemy ) )
		{
			targets.AddRange( BattleManager.GetAliveUnits( user.Faction.GetOpposite() ) );
		}

		if ( targeting.HasFlag( CardTargets.Ally ) )
		{
			targets.AddRange( BattleManager.GetAliveUnits( user.Faction ) );
		}

		return targets.Where( u => u.IsValid() ).ToList();
	}

	private void TriggerBeforePlayEffects( BattleUnitComponent owner, BattleUnitComponent target )
	{
		if ( ActiveEffect is not null )
		{
			var detail = CreateCardDetail( owner, target );
			ActiveEffect.BeforePlay( detail );
		}
		
		foreach ( var status in owner.StatusEffects?.ToList() ?? [] )
		{
			status.BeforePlayCard( this );
		}

		foreach ( var passive in owner.Passives?.ToList() ?? [] )
		{
			passive.BeforePlayCard( this );
		}

		if ( RelicManager.Instance.IsValid() )
		{
			foreach ( var relic in RelicManager.Instance.Relics )
			{
				relic.BeforePlayCard( this, owner );
			}
		}
	}

	private void TriggerOnPlayEffects( BattleUnitComponent owner, BattleUnitComponent target )
	{
		if ( ActiveEffect is not null ) 
		{ 
			var detail = CreateCardDetail( owner, target ); 
			ActiveEffect.OnPlay( detail );
		}
			
		foreach ( var status in owner.StatusEffects?.ToList() ?? [] )
		{
			status.OnPlayCard( this );
		}

		foreach ( var passive in owner.Passives?.ToList() ?? [] )
		{
			passive.OnPlayCard( this );
		}

		if ( RelicManager.Instance.IsValid() )
		{
			foreach ( var relic in RelicManager.Instance.Relics )
			{
				relic.OnPlayCard( this, owner );
			}
		}
	}

	private static void TriggerDamageEvents( BattleUnitComponent attacker, BattleUnitComponent target )
	{
		foreach ( var status in target.StatusEffects?.ToList() ?? [] )
		{
			status.OnTakeDamage( attacker );
		}

		foreach ( var status in attacker.StatusEffects?.ToList() ?? [] )
		{
			status.OnDealDamage( target );
		}
	}

	private static Effects.CardEffect.CardEffectDetail CreateCardDetail( BattleUnitComponent owner, BattleUnitComponent? target = null )
	{
		return new Effects.CardEffect.CardEffectDetail
		{
			Unit = owner, Target = target
		};
	}
	
	private static Effects.ActionEffect.ActionEffectDetail CreateActionDetail( BattleUnitComponent owner, BattleUnitComponent? target = null )
	{
		return new Effects.ActionEffect.ActionEffectDetail
		{
			Unit = owner, Target = target
		};
	}

	/// <summary>
	/// Creates a deep copy of this card.
	/// </summary>
	public Card DeepCopy()
	{
		var card = new Card
		{
			Id = Id,
			Name = Name,
			Cost = Cost,
			IsInstant = IsInstant,
			Targets = Targets,
			Targeting = Targeting,
			Type = Type,
			Rarity = Rarity,
			Availabilities = Availabilities,
			Keywords = Keywords,
			Effect = Effect,
			ActiveEffect = ActiveEffect,
			Actions = Actions.Select( x => x.DeepCopy() ).ToList()
		};

		return card;
	}

	public override string ToString()
	{
		return $"Card: {Name} - Id: {Id.LocalId}";
	}

	public enum CardType
	{
		Attack,
		Spell,
		Defense,
		Item
	}

	public enum CardRarity
	{
		Common,
		Uncommon,
		Rare,
		Epic
	}

	[Flags]
	public enum CardTargets
	{
		None = 0,
		Enemy = 1 << 0,
		Self = 1 << 1,
		Ally = 1 << 2,
		All = Enemy | Self | Ally
	}

	[Flags]
	public enum CardAvailabilities
	{
		None = 0,
		[Description( "Found in shops" )]
		Shop = 1 << 0,
		[Description( "In starting deck" )]
		Starter = 1 << 1,
		[Description( "Given as a reward" )]
		Reward = 1 << 2,
		[Description( "Found in chests" )]
		Chest = 1 << 3,
		[Description( "Granted in special events" )]
		Event = 1 << 4,
		[Description( "Only available in dev builds/testing" )]
		DevOnly = 1 << 5,
	}

	public class CardEffect
	{
		public string Script { get; set; } = string.Empty;
		
		[InlineEditor]
		public RangedInt Power { get; set; } = 1;
	}
}
