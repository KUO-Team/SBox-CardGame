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

	[InlineEditor]
	public CardCost Cost { get; set; } = new();

	public bool IsInstant { get; set; }

	public bool IsIndiscriminate { get; set; }

	public int Targets { get; set; } = 1;

	public CardTargets Targeting { get; set; } = CardTargets.Enemy;

	public CardType Type { get; set; } = CardType.Attack;

	public CardRarity Rarity { get; set; } = CardRarity.Common;

	public CardAvailabilities Availabilities { get; set; } = CardAvailabilities.None;

	public TagSet Keywords { get; set; } = new();

	[InlineEditor, WideMode]
	public List<Action> Actions { get; set; } = [];

	[Hide, JsonIgnore]
	public CardModifiers Modifiers { get; } = new();

	/// <summary>
	/// The cost you actually have to pay
	/// </summary>
	[Hide, JsonIgnore]
	public CardCost EffectiveCost
	{
		get
		{
			var delta = Modifiers.GetCostDelta();
			return new CardCost
			{
				Ep = Math.Max( 0, Cost.Ep + delta.Ep ), Mp = Math.Max( 0, Cost.Mp + delta.Mp )
			};
		}
	}

	private static readonly Logger Log = new( "Card" );

	/// <summary>
	/// Plays the card on the specified target using the given slot.
	/// </summary>
	/// <param name="target">The target to apply the card effects on.</param>
	/// <param name="slot">The slot from which the card is played.</param>
	public void Play( BattleUnit target, CardSlot slot )
	{
		var owner = slot.Owner;
		if ( !owner.IsValid() || !owner.HealthComponent.IsValid() || owner.HealthComponent.IsDead )
		{
			Log.Warning( "Invalid or dead owner tried to play a card." );
			return;
		}

		var selectedTargets = SelectTargets( target, owner );
		TriggerOnPlayEffects( owner );

		foreach ( var selected in selectedTargets )
		{
			PlayOnTarget( owner, selected );
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
		TriggerAfterPlayEffects( owner, target );
	}

	private void PlayOnTarget( BattleUnit owner, BattleUnit target )
	{
		foreach ( var action in Actions )
		{
			var basePower = action.EffectivePower.Value;
			var modifiedPower = basePower + TriggerPowerEffects( owner, this, action );

			var effect = action.Effect;
			if ( effect is not null )
			{
				effect.Card = this;
				effect.Power = modifiedPower;
				effect.OnPlay( CreateDetail( owner, target ) );
			}

			TriggerDamageEvents( owner, target );
			var damage = modifiedPower;
			if ( effect is not null )
			{
				damage += effect.DamageModifier( CreateDetail( owner, target ) );
			}

			Log.EditorLog( $"{Name} basePower: {basePower}, modified: {modifiedPower}, final damage: {damage}" );
			if ( action.Type == Action.ActionType.Attack )
			{
				target.HealthComponent?.TakeDamage( damage, owner );
			}
		}
	}

	private List<BattleUnit> SelectTargets( BattleUnit mainTarget, BattleUnit owner )
	{
		var allValid = GetValidTargets( owner, Targeting ).Where( u => u != mainTarget ).ToList();
		var targets = new List<BattleUnit>
		{
			mainTarget
		};

		if ( Targets > 1 )
		{
			targets.AddRange( allValid.OrderBy( _ => Game.Random.Next() ).Take( Targets - 1 ) );
		}

		return targets;
	}

	private static List<BattleUnit> GetValidTargets( BattleUnit user, CardTargets targeting )
	{
		var targets = new List<BattleUnit>();

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

	private void TriggerOnPlayEffects( BattleUnit owner )
	{
		foreach ( var status in owner.StatusEffects?.ToList() ?? [] )
		{
			status.OnPlayCard( this );
		}

		if ( RelicManager.Instance.IsValid() )
		{
			foreach ( var relic in RelicManager.Instance.Relics )
			{
				relic.OnPlayCard( this, owner );
			}
		}
	}

	private void TriggerAfterPlayEffects( BattleUnit owner, BattleUnit target )
	{
		foreach ( var action in Actions )
		{
			if ( action.Effect is not {} effect )
			{
				continue;
			}

			effect.Card = this;
			effect.AfterPlay( CreateDetail( owner, target ) );
		}

		foreach ( var status in owner.StatusEffects?.ToList() ?? [] )
		{
			status.AfterPlayCard( this );
		}

		if ( RelicManager.Instance.IsValid() )
		{
			foreach ( var relic in RelicManager.Instance.Relics )
			{
				relic.AfterPlayCard( this, owner );
			}
		}
	}

	private int TriggerPowerEffects( BattleUnit source, Card card, Action action )
	{
		if ( action.Type == Action.ActionType.Defense )
		{
			return 0;
		}

		var contribution = 0;
		contribution += Modifiers.GetPowerDelta( action );
		foreach ( var status in source.StatusEffects?.ToList() ?? [] )
		{
			contribution += status.PowerModifier( card, action );
		}

		foreach ( var passive in source.Passives?.ToList() ?? [] )
		{
			contribution += passive.PowerModifier( card, action );
		}

		return contribution;
	}

	private void TriggerDamageEvents( BattleUnit attacker, BattleUnit target )
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

	private static CardEffect.CardEffectDetail CreateDetail( BattleUnit owner, BattleUnit? target = null )
	{
		return new CardEffect.CardEffectDetail
		{
			Unit = owner, Target = target
		};
	}

	/// <summary>
	/// Creates a deep copy of this card.
	/// </summary>
	public Card DeepCopy()
	{
		var card = new Card()
		{
			Id = Id,
			Name = Name,
			Cost = Cost,
			IsInstant = IsInstant,
			IsIndiscriminate = IsIndiscriminate,
			Targets = Targets,
			Targeting = Targeting,
			Type = Type,
			Rarity = Rarity,
			Availabilities = Availabilities,
			Keywords = Keywords,
			Actions = Actions.Select( x => x.DeepCopy() ).ToList()
		};

		return card;
	}

	public override string ToString()
	{
		return $"Card: {Name} - Id: {Id.LocalId}";
	}

	public class CardCost
	{
		public int Ep { get; set; }
		public int Mp { get; set; }
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
}
