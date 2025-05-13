using System;

namespace CardGame.Controllers;

public sealed class PlayerController : Base2DController
{
	[Property, ToggleGroup( "CanRun" ), Sync]
	public float RunSpeed { get; set; } = 150f;

	[Property, ToggleGroup( "CanRun" ), Sync]
	public bool CanRun { get; set; }

	[Property, ToggleGroup( "CanRun" ), InputAction, Sync]
	public string RunInput { get; set; } = "run";

	[Property]
	public Vector3 CameraOffset { get; set; }

	// Acceleration for smoother movement
	[Property]
	public float Acceleration { get; set; } = 10f;

	// Deceleration when stopping
	[Property]
	public float Friction { get; set; } = 15f;

	protected override void OnUpdate()
	{
		UpdateAnimations();
	}

	protected override void OnFixedUpdate()
	{
		UpdateCamera();
		UpdateMovement();
		//ApplyBounceEffect();
		Move();
	}

	private void ApplyBounceEffect()
	{
		// If we're moving, start a small bounce
		if ( IsMoving )
		{
			_bounceVelocity = MathX.Lerp( _bounceVelocity, BounceAmount, BounceSpeed * Time.Delta );
		}
		else
		{
			// If we stop, let the bounce settle slowly
			_bounceVelocity = MathX.Lerp( _bounceVelocity, 0, BounceSpeed * Time.Delta );
		}

		// Use a sine function for smooth, less bouncy oscillation
		_bounceOffset = MathF.Sin( Scene.FixedDelta * 5 ) * _bounceVelocity;

		GameObject.WorldPosition = new Vector3( GameObject.WorldPosition.x, GameObject.WorldPosition.y + _bounceOffset, GameObject.WorldPosition.z );
	}

	// Bounce settings
	[Property]
	public float BounceAmount { get; set; } = 1f; // The height of the bounce

	[Property]
	public float BounceSpeed { get; set; } = 2f; // How fast the bounce happens

	private float _bounceOffset = 0f; // Current bounce offset
	private float _bounceVelocity = 0f; // Velocity of bounce (used for smooth damping)

	private void UpdateCamera()
	{
		if ( !Camera.IsValid() )
		{
			return;
		}
		
		var p = new Vector3(
			GameObject.WorldPosition.x + CameraOffset.x,
			GameObject.WorldPosition.y + CameraOffset.y,
			Camera.WorldPosition.z
		);

		Camera.WorldPosition = p;
	}

	private void UpdateMovement()
	{
		if ( !CanWalk )
		{
			Velocity = Vector2.Zero;
			return;
		}

		var input = Input.AnalogMove.Normal;

		if ( input.Length > 0 )
		{
			// Normalize so diagonals aren't faster
			input = input.Normal;

			var desiredVelocity = new Vector2( -input.y, input.x );

			var targetSpeed = CanRun && Input.Down( RunInput )
				? RunSpeed
				: WalkSpeed;

			desiredVelocity *= targetSpeed;

			// Lerp toward desired velocity for smooth acceleration
			Velocity = Vector2.Lerp( Velocity, desiredVelocity, Acceleration * Time.Delta );
		}
		else
		{
			// Smoothly come to a stop
			Velocity = Vector2.Lerp( Velocity, Vector2.Zero, Friction * Time.Delta );
		}
	}

	protected override void UpdateAnimations()
	{
		if ( Sprite == null )
			return;

		// Update last non-zero velocity
		if ( IsMoving )
		{
			LastNonZeroVelocity = Velocity;
			PendingIdleTransition = false; // Cancel idle transition if we moved again
			AnimationCooldown = 0f;
		}
		else
		{
			AnimationCooldown += Time.Delta;
		}

		var desiredAnimation = GetAnimation();

		// Handle transition from walk -> idle with optional easing
		if ( !IsMoving && LastAnimation == "walk" && desiredAnimation == "idle" )
		{
			if ( !PendingIdleTransition )
			{
				PendingIdleTransition = true;
				AnimationCooldown = 0f;
				return;
			}

			if ( PendingIdleTransition && AnimationCooldown < TransitionToIdleDelay )
			{
				// Wait a bit before switching to idle
				return;
			}
		}

		// Only switch animation if it's actually changed
		if ( LastAnimation != desiredAnimation )
		{
			Sprite.PlayAnimation( desiredAnimation );
			LastAnimation = desiredAnimation;
		}

		// Direction
		Sprite.SpriteFlags = LastNonZeroVelocity.x >= 0
			? SpriteFlags.HorizontalFlip
			: SpriteFlags.None;

		// Playback speed
		var effectiveSpeed = CanRun && Input.Down( RunInput ) ? RunSpeed : WalkSpeed;
		Sprite.PlaybackSpeed = IsMoving ? Velocity.Length / effectiveSpeed : 1;
	}

}
