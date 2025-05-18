using CardGame.Passives;

namespace CardGame.Units;

public class PassiveAbilityList : OwnableListComponent<PassiveAbility>
{
	public void AddPassiveAbility( Data.PassiveAbility data )
	{
		var passive = TypeLibrary.Create<PassiveAbility>( data.Script, [data] );
		passive.Owner = Owner;
		passive.OnAdd();
		Add( passive );
	}
}
