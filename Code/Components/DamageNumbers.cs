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

	public TimeUntil Lifetime { get; set; } = 1f;
	
	private Vector3 _initialPosition;

	public void Init( GameObject target )
	{
		Target = target;
		_initialPosition = target.WorldPosition;
	}

	protected override void OnFixedUpdate()
	{
		if ( !Target.IsValid() )
		{
			return;
		}

		GameObject.WorldPosition = _initialPosition + new Vector3( 0, MoveSpeed * Lifetime.Passed, 2 );

		//FadeText();

		if ( Lifetime )
		{
			GameObject.Destroy();
		}
		
		base.OnFixedUpdate();
	}

	private void FadeText()
	{
		if ( !Text.IsValid() )
		{
			return;
		}
		
		var alpha = 1 - Lifetime.Passed * FadeSpeed;
		alpha = Math.Clamp( alpha, 0, 1 );
		var color = Text.Color;
		color.a = alpha;
		Text.Color = color;
	}
}
