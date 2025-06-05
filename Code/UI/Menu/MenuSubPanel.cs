using Sandbox.UI;

namespace CardGame.UI;

public abstract class MenuSubPanel : Panel
{
	public MainMenu? Menu { get; set; }
	
	protected T? GetMenuSubPanel<T>() where T : MenuSubPanel
	{
		return !Menu.IsValid() ? null : Menu.GetSubPanel<T>();
	}
}
