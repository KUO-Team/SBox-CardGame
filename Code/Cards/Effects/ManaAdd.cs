namespace CardGame.Effects;

public class ManaAdd( Card card, RangedInt power ) : CardEffect( card, power )
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
