namespace CardGame.Effects;

public class ManaRecover( Card card ) : CardEffect( card )
{
	public override string Description => "Recover @ MP";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		if ( !detail.Unit.IsValid() )
		{
			return;
		}

		detail.Unit.RecoverMana( Power );
		base.OnPlay( detail );
	}
}
