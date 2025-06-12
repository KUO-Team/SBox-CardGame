using System;
using System.Text.Json.Serialization;
using Sandbox.Diagnostics;
using CardGame.Units;

namespace CardGame;

public class Action : IDeepCopyable<Action>
{
	public ActionType Type { get; set; } = ActionType.Attack;

	[InlineEditor]
	public RangedInt Power { get; set; } = 1;

	[Hide, JsonIgnore]
	public RangedInt EffectivePower
	{
		get
		{
			if ( Card is null )
			{
				return Power;
			}

			var delta = Card.Modifiers.GetActionPowerDelta( this );
			return new RangedInt( Math.Max( 0, Power.Min + delta ), Math.Max( 0, Power.Max + delta ) );
		}
	}

	[Title( "Action Effect" )]
	[InlineEditor]
	public ActionEffect Effect { get; set; } = new();

	[Hide, JsonIgnore]
	public Effects.ActionEffect? ActiveEffect { get; set; }

	[Hide, JsonIgnore]
	public Card? Card { get; set; }

	private static readonly Logger Log = new( "Action" );

	public void InitEffect()
	{
		if ( string.IsNullOrWhiteSpace( Effect.Script ) )
		{
			return;
		}

		ActiveEffect = TypeLibrary.Create<Effects.ActionEffect>( Effect.Script, [Card, this, Effect.Power] );
	}

	public int GetPowerRoll( BattleUnitComponent source, Card card )
	{
		var power = EffectivePower.Value;
		var modifiedPower = power + TriggerPowerEffects( source, card );

		Log.EditorLog( $"{Type} Action Rolled | Base Power: {power} | Modified Power: {modifiedPower}" );
		return modifiedPower;
	}

	private int TriggerPowerEffects( BattleUnitComponent source, Card card )
	{
		var contribution = 0;
		contribution += card.Modifiers.GetActionPowerDelta( this );
		foreach ( var status in source.StatusEffects?.ToList() ?? [] )
		{
			contribution += status.PowerModifier( card, this );
		}

		foreach ( var passive in source.Passives?.ToList() ?? [] )
		{
			contribution += passive.PowerModifier( card, this );
		}

		return contribution;
	}

	public Action DeepCopy()
	{
		var action = new Action
		{
			Type = Type, 
			Power = Power,
			Effect = Effect,
			ActiveEffect = ActiveEffect,
			Card = Card
		};

		return action;
	}

	public enum ActionType
	{
		Attack,
		Defense
	}

	public class ActionEffect
	{
		public string Script { get; set; } = string.Empty;

		[InlineEditor]
		public RangedInt Power { get; set; } = 1;
	}
}
