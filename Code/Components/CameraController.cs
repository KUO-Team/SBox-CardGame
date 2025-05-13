using System;

namespace CardGame;

public sealed class CameraController : Component
{
	[Property]
	public float ZoomSpeed { get; set; } = 0.5f;

	[Property]
	public float MinZoom { get; set; } = 5.0f;

	[Property]
	public float MaxZoom { get; set; } = 15.0f;

	[Property]
	public float DragSpeed { get; set; } = 1f;

	[Property]
	public float MaxHorizontalOffset { get; set; } = 10.0f;

	[Property]
	public float MaxVerticalOffset { get; set; } = 10.0f;

	[Property, Category( "State" ), ReadOnly]
	public bool IsDragging { get; private set; }

	private Vector3 _initialPosition;
	private Vector2 _lastMousePosition;
	private float _currentZoom;
	private Vector2 _cameraOffset = Vector2.Zero;

	protected override void OnStart()
	{
		if ( Scene.Camera.IsValid() )
		{
			_initialPosition = Scene.Camera.WorldPosition;
			_currentZoom = Scene.Camera.FieldOfView;
		}
		else
		{
			Log.Warning( "CameraController: Scene Camera not found" );
		}
	}

	protected override void OnUpdate()
	{
		if ( !Scene.Camera.IsValid() )
		{
			return;
		}

		HandleZoom( Scene.Camera );
		HandleDrag( Scene.Camera );
	}

	private void HandleZoom( CameraComponent camera )
	{
		var mouseWheelDelta = Input.MouseWheel.y;

		if ( !(Math.Abs( mouseWheelDelta ) > 0.01f) )
		{
			return;
		}

		// Negative wheel = zoom out (increase FOV), Positive wheel = zoom in (decrease FOV)
		_currentZoom = Math.Clamp(
			_currentZoom - mouseWheelDelta * ZoomSpeed,
			MinZoom,
			MaxZoom
		);

		camera.FieldOfView = _currentZoom;
	}

	private void HandleDrag( CameraComponent camera )
	{
		if ( Input.Pressed( "attack1" ) )
		{
			IsDragging = true;
			_lastMousePosition = Mouse.Position;
			Mouse.CursorType = "grab";
		}

		if ( Input.Released( "attack1" ) )
		{
			IsDragging = false;
		}

		if ( !IsDragging )
		{
			return;
		}

		var currentMousePosition = Mouse.Position;
		var mouseDelta = currentMousePosition - _lastMousePosition;

		_cameraOffset.x -= mouseDelta.x * DragSpeed * 0.01f;
		_cameraOffset.y += mouseDelta.y * DragSpeed * 0.01f;

		// Clamp offsets within limits
		_cameraOffset.x = Math.Clamp(
			_cameraOffset.x,
			-MaxHorizontalOffset,
			MaxHorizontalOffset
		);

		_cameraOffset.y = Math.Clamp(
			_cameraOffset.y,
			-MaxVerticalOffset,
			MaxVerticalOffset
		);

		// Apply offset to camera position
		var newPosition = _initialPosition;
		newPosition.x = _initialPosition.x + _cameraOffset.x;
		newPosition.y = _initialPosition.y + _cameraOffset.y;

		camera.WorldPosition = newPosition;
		_lastMousePosition = currentMousePosition;
	}
}
