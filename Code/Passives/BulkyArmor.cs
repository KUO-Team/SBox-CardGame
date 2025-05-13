using CardGame.Data;
using CardGame.StatusEffects;

namespace CardGame.Passives;

public class BulkyArmor : PassiveAbility
{
	public override Id Id => 2;
	public override string Name => "Bulky Armor";
	public override string Description => "At the start of the turn, gain 2 Protection.";

	public override void OnTurnStart()
	{
		Owner?.StatusEffects?.AddStatusEffect<Protection>( 2 );
		base.OnTurnStart();
	}
}
