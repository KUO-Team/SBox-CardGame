using System;

namespace CardGame;

public sealed class DamageNumbers : Component
{
	[Property, ReadOnly]
	public GameObject? Target { get; set; }

	[Property, RequireComponent] 
	public TextRenderer? Text { get; set; }

	[Property] 
	public float MoveSpeed { get; set; } = 10f;
	
	[Property] 
	public float FadeSpeed { get; set; } = 2f;

	private float _timer = 0f;
	private Vector3 _initialPosition;

	public void Init( GameObject target )
	{
		Target = target;
		_initialPosition = new Vector3( target.WorldPosition.x, target.WorldPosition.y, WorldPosition.z );
	}

	protected override void OnUpdate()
	{
		if ( !Target.IsValid() )
		{
			return;
		}

		_timer += Time.Delta;
		GameObject.WorldPosition = _initialPosition + new Vector3( 0, MoveSpeed * _timer, WorldPosition.z );

		// Gradually fade the text
		if ( Text.IsValid() )
		{
			var alpha = 1 - _timer * FadeSpeed;
			alpha = Math.Clamp( alpha, 0, 1 );
		}

		// Destroy the damage number after it fades out completely
		if ( _timer > 2 / FadeSpeed )
		{
			GameObject.Destroy();
		}
		base.OnUpdate();
	}
}
