using System;

namespace CardGame.UI;

/// <summary>
/// Marks a panel to be created on the hud when <see cref="Hud.OnStart"/> is called.
/// </summary>
[AttributeUsage( AttributeTargets.Class )]
public class HudAttribute : Attribute, ITypeAttribute
{
	public Type? TargetType { get; set; }

	public bool Hidden { get; set; }

	public HudAttribute() { }

	/// <param name="hidden">If this panel is hidden by default.</param>
	public HudAttribute( bool hidden ) : this()
	{
		Hidden = hidden;
	}
}
