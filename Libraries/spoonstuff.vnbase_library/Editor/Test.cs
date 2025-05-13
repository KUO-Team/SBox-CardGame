using Editor;
using Sandbox;
using SandLang;

public class MyClass
{
	public Dialogue.Label? Label { get; set; }
}

[CustomEditor(typeof(MyClass))]
public class ScriptHelperWidget : ControlObjectWidget
{
	public ScriptHelperWidget( SerializedProperty property, bool create ) : base( property, create )
	{
		Layout = Layout.Row();
		Layout.Add( new Label( "Script Helper" ) );
	}
}
