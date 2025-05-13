using CardGame.UI;
using WorldInput=Sandbox.UI.WorldInput;

namespace CardGame;

public class InputComponent : Singleton<InputComponent>
{
	public required WorldInput WorldInput { get; init; } = new();

	public static CardSlot? SelectedSlot { get; set; }

	public static Vector3 SelectedSlotPosition { get; set; }

	public static System.Action<CardSlot>? OnSelect { get; set; }

	private Ray _mouseRay;
	
	protected override void OnStart()
	{
		SelectedSlot = null;
		SelectedSlotPosition = default;
		HandPanel.SelectedCard = null;
		base.OnStart();
	}

	protected override void OnDestroy()
	{
		foreach ( var @delegate in OnSelect?.GetInvocationList() ?? [] )
		{
			OnSelect -= @delegate as System.Action<CardSlot>;
		}

		base.OnDestroy();
	}

	protected override void OnUpdate()
	{
		if ( !Scene.Camera.IsValid() )
		{
			return;
		}
		
		var mousePosition = Mouse.Position;
		_mouseRay = Scene.Camera.ScreenPixelToRay( mousePosition );

		WorldInput.Ray = _mouseRay;
		WorldInput.MouseLeftPressed = Sandbox.Input.Pressed( "attack1" );
		WorldInput.MouseRightPressed = Sandbox.Input.Pressed( "attack2" );

		base.OnUpdate();
	}
}
