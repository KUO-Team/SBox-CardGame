using SpriteTools;

namespace CardGame.Controllers;

public abstract class Base2DController : Component
{
	[Property, RequireComponent]
	public SpriteComponent? Sprite { get; set; }

	[Property, Sync]
	public bool CanWalk { get; set; } = true;

	[Property, Sync]
	public float WalkSpeed { get; set; } = 100f;

	[Property]
	public bool IsMoving => Velocity.Length > MovementThreshold;

	[Property, ReadOnly, Sync]
	public Vector2 Velocity { get; set; } = Vector2.Zero;

	protected Vector2 LastNonZeroVelocity = Vector2.Zero;

	protected CameraComponent? Camera => Scene.Camera;

	protected float AnimationCooldown = 0f;
	protected const float MovementThreshold = 0.1f;
	protected const float TransitionToIdleDelay = 0.15f;

	protected string LastAnimation = "";
	protected bool PendingIdleTransition = false;

	protected virtual string GetAnimation()
	{
		return IsMoving ? "walk" : "idle";
	}

	protected virtual void UpdateAnimations()
	{
		if ( !Sprite.IsValid() )
		{
			return;
		}

		if ( IsMoving )
		{
			LastNonZeroVelocity = Velocity;
		}

		var animation = GetAnimation();

		// Only play animation if it's not already playing
		if ( Sprite.CurrentAnimation.Name != animation )
		{
			Sprite.PlayAnimation( animation );
		}

		// Determine sprite direction based on velocity or last direction
		Sprite.SpriteFlags = LastNonZeroVelocity.x >= 0 ? SpriteFlags.HorizontalFlip : SpriteFlags.None;

		// Adjust playback speed based on effective speed
		Sprite.PlaybackSpeed = IsMoving ? Velocity.Length / WalkSpeed : 1;
	}

	protected virtual void Move()
	{
		WorldPosition += new Vector3( Velocity.x, Velocity.y, 0 ) * Time.Delta;
	}
}
