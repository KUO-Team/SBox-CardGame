using System;
using CardGame.Characters;

namespace CardGame.UI;

public partial class InteractionPrompt
{
	[Property, InputAction]
	public string InteractKey { get; set; } = "interact";
	
	[Property]
	public InteractType Type { get; set; }
	
	[Property, ShowIf( nameof( Type ), InteractType.Dialogue )]
	public DialogueComponent? DialogueComponent { get; set; }

	public enum InteractType
	{
		None,
		Dialogue
	}

	public bool CanInteract { get; set; }
	
	public void OnTriggerEnter( Collider collider )
	{
		if ( !collider.Tags.Has( "player" ) )
		{
			return;
		}

		CanInteract = true;
	}
	
	public void OnTriggerExit( Collider collider )
	{
		if ( !collider.Tags.Has( "player" ) )
		{
			return;
		}

		CanInteract = false;
	}
	
	protected override void OnFixedUpdate()
	{
		if ( !CanInteract || !PlayerCharacter.Local.CanInteract )
		{
			return;
		}

		switch ( Type )
		{
			case InteractType.None:
				break;
			case InteractType.Dialogue:
				{
					if ( Input.Pressed( InteractKey ) )
					{
						DialogueComponent?.Interact();
					}
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		base.OnFixedUpdate();
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( InteractKey, DialogueComponent, CanInteract, PlayerCharacter.Local?.CanInteract );
	}
}
