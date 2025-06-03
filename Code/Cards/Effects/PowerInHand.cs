using CardGame.Modifiers;

namespace CardGame.Effects;

public class PowerInHand( Card card ) : CardEffect( card )
{
	public override string Description => $"Gain power equal to the amount of cards named {Card.Name} in hand";

	private PowerModifier? _modifier;

	public override void BeforePlay( CardEffectDetail detail )
	{
		if ( !detail.Unit.IsValid() )
		{
			return;
		}

		if ( !detail.Unit.HandComponent.IsValid() )
		{
			return;
		}
		
		var amount = detail.Unit.HandComponent.Hand.Count( x => x.Name == Card.Name );
		if ( amount <= 0 )
		{
			return;
		}

		_modifier = new PowerModifier( amount, action => action.Type == Action.ActionType.Attack, 1 );
		Card.Modifiers.AddModifier( _modifier );
		
		base.BeforePlay( detail );
	}

	public override void OnPlay( CardEffectDetail detail )
	{
		if ( _modifier is null )
		{
			return;
		}
		
		Card.Modifiers.RemoveModifier( _modifier );
		_modifier = null;
		base.OnPlay( detail );
	}
}
