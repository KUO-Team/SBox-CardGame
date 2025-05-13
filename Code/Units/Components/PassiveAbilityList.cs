using CardGame.Passives;

namespace CardGame.Units;

public class PassiveAbilityList : OwnableListComponent<PassiveAbility>
{
	public void AddPassiveAbility<T>() where T : PassiveAbility, new()
	{
		var passive = new T
		{
			Owner = Owner
		};

		passive.OnAdd();
		Add( passive );
	}

	public void AddPassiveAbility( PassiveAbility passive )
	{
		passive.OnAdd();
		Add( passive );
	}
}
