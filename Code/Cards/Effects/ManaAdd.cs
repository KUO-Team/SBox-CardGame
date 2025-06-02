namespace CardGame.Effects;

public class ManaAdd( Card card ) : CardEffect( card )
{
	public override string Description => "Add @ MP";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		if ( !detail.Unit.IsValid() )
		{
			return;
		}

		detail.Unit.Mana += Power;
		base.OnPlay( detail );
	}
}
