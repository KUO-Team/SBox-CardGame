﻿using System;

namespace CardGame.UI;

public partial class InputGlyph
{
	private string? _inputAction;
	private InputAnalog _inputAnalog;
	private InputGlyphSize _inputGlyphSize;
	private bool _outline;
	
	protected override void OnAfterTreeRender( bool firstTime )
	{
		Update();
	}
	
	public void SetAction( string inputAction )
	{
		if ( _inputAction == inputAction )
		{
			return;
		}

		_inputAction = inputAction;
		BindClass( "pressed", () => Input.Down( _inputAction ) );
		Update();
	}
	
	public void SetAnalog( string inputAnalog )
	{
		if ( !Enum.TryParse<InputAnalog>( inputAnalog, true, out var analog ) )
		{
			return;
		}

		switch ( analog )
		{
			case InputAnalog.LeftStickX:
			case InputAnalog.LeftStickY:
				{
					BindClass( "pressed", () => !Input.AnalogMove.IsNearlyZero() );
					break;
				}
			case InputAnalog.RightStickX:
			case InputAnalog.RightStickY:
				{
					BindClass( "pressed", () => !Input.AnalogLook.IsNearlyZero() );
					break;
				}
			case InputAnalog.LeftTrigger:
			case InputAnalog.RightTrigger:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		_inputAnalog = analog;
	}
	
	public override void SetProperty( string name, string value )
	{
		switch ( name )
		{
			case "action":
				{
					SetAction( value );
					Update();

					break;
				}

			case "analog":
				{
					SetAnalog( value );
					Update();

					break;
				}

			case "size":
				{
					Enum.TryParse( value, true, out _inputGlyphSize );
					Update();

					break;
				}

			case "outline":
				{
					_outline = value switch
					{
						"true" => true,
						"false" => false,
						_ => _outline
					};

					Update();
					break;
				}
		}

		base.SetProperty( name, value );
	}
	
	private void Update()
	{
		var texture = _inputAction is null ? Input.GetGlyph( _inputAnalog, _inputGlyphSize, _outline ) : Input.GetGlyph( _inputAction, _inputGlyphSize, _outline );
		Style.BackgroundImage = texture;
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( Input.UsingController );
	}
}
