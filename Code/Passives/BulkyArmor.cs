using CardGame.Data;
using CardGame.StatusEffects;

namespace CardGame.Passives;

public class BulkyArmor( Data.PassiveAbility data ) : PassiveAbility( data )
{
	public override void OnTurnStart()
	{
		Owner?.StatusEffects?.AddStatusEffect<Protection>( 2 );
		base.OnTurnStart();
	}
}
