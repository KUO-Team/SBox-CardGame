namespace CardGame.Effects;

public class StunnedNextTurn( Card card ) : CardEffect( card )
{
	public override string Description => "Become Stunned Next Turn";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.StatusEffects?.AddStatusEffectByKeyNextTurn( StatusEffects.StatusEffect.StatusKey.Stunned );
		base.OnPlay( detail );
	}
}
