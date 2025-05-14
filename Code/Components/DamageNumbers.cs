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

	private float _timer;
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

		_timer += Time.Delta;
		GameObject.WorldPosition = _initialPosition + new Vector3( 0, MoveSpeed * _timer, 2 );

		//FadeText();

		// Destroy the damage number after it fades out completely
		if ( _timer > 2 / FadeSpeed )
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
		
		var alpha = 1 - _timer * FadeSpeed;
		alpha = Math.Clamp( alpha, 0, 1 );
		var color = Text.Color;
		color.a = alpha;
		Text.Color = color;
	}
}
