using CardGame.UI.Map;

namespace CardGame.Events;

public class Shop( Data.Event data ) : Event( data )
{
	public override void OnChoiceSelected( Data.Event.Choice choice, int index )
	{
		switch ( index )
		{
			case 0:
				{
					if ( !Panel.IsValid() )
					{
						return;
					}

					var mapPanel = Game.ActiveScene.GetInstance<MapPanel>();
					if ( mapPanel.IsValid() )
					{
						mapPanel.ShowShop();
					}
					break;
				}
			case 1:
				{
					break;
				}
		}
		
		base.OnChoiceSelected( choice, index );
	}
}
