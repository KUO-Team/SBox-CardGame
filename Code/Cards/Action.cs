using System;
using System.Text.Json.Serialization;
using CardGame.Effects;
using Sandbox.Diagnostics;

namespace CardGame;

public class Action : IDeepCopyable<Action>
{
	public ActionType Type { get; set; } = ActionType.Attack;

	[InlineEditor]
	public RangedInt Power { get; set; } = 1;

	public string Script { get; set; } = string.Empty;

	[Hide, JsonIgnore]
	public RangedInt EffectivePower
	{
		get
		{
			if ( Card is null )
			{
				return Power;
			}

			var delta = Card.Modifiers.GetPowerDelta( this );
			return new RangedInt( Math.Max( 0, Power.Min + delta ), Math.Max( 0, Power.Max + delta ) );
		}
	}

	[Hide, JsonIgnore]
	public CardEffect? Effect { get; set; }

	[Hide, JsonIgnore]
	public Card? Card { get; set; }

	private static readonly Logger Log = new( "Action" );

	public void InitEffect()
	{
		if ( string.IsNullOrWhiteSpace( Script ) )
		{
			return;
		}

		if ( Type != ActionType.Effect )
		{
			Log.Warning( $"Can't assign effect on non-effect type action!" );
			return;
		}

		Effect = TypeLibrary.Create<CardEffect>( Script, [Card] );
	}

	public Action DeepCopy()
	{
		var action = new Action
		{
			Type = Type,
			Power = Power,
			Script = Script,
			Effect = Effect,
			Card = Card
		};

		return action;
	}

	public enum ActionType
	{
		Attack,
		Defense,
		Effect
	}
}
