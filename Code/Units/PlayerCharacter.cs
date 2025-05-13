using CardGame.Controllers;

namespace CardGame.Characters;

public class PlayerCharacter : BaseCharacter
{
	public static PlayerCharacter Local { get; private set; } = null!;

	[Property]
	public bool CanInteract { get; set; } = true;
	
	[Property]
	public Base2DController? Controller { get; set; }

	protected override void OnStart()
	{
		if ( IsProxy )
		{
			return;
		}

		Local = this;
		
		base.OnStart();
	}
}
