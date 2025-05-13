using VNBase;
using VNBase.Assets;
using CardGame.Characters;

namespace CardGame;

public class DialogueComponent : Component
{
	[Property, TextArea]
	public string DialogueText { get; set; } = string.Empty;
	
	public void Interact()
	{
		var script = new Script()
		{
			Dialogue = DialogueText
		};

		var scriptPlayer = ScriptPlayer.Instance;
		if ( scriptPlayer.IsValid() )
		{
			scriptPlayer.OnScriptLoad += OnScriptLoad;
			scriptPlayer.OnScriptUnload += OnScriptUnload;
			scriptPlayer.LoadScript( script );
		}
	}

	private void OnScriptLoad( Script script )
	{
		var character = PlayerCharacter.Local;
		if ( character.IsValid() )
		{
			character.CanInteract = false;
			if ( character.Controller.IsValid() )
			{
				character.Controller.CanWalk = false;
				character.Controller.Velocity = new Vector2();
			}
		}
	}
	
	private void OnScriptUnload( Script script )
	{
		var character = PlayerCharacter.Local;
		if ( character.IsValid() )
		{
			character.CanInteract = true;
			if ( character.Controller.IsValid() )
			{
				character.Controller.CanWalk = true;
				character.Controller.Velocity = new();
			}
		}
		
		var scriptPlayer = ScriptPlayer.Instance;
		if ( scriptPlayer.IsValid() )
		{
			scriptPlayer.OnScriptLoad -= OnScriptLoad;
			scriptPlayer.OnScriptUnload -= OnScriptUnload;
		}
	}
}
