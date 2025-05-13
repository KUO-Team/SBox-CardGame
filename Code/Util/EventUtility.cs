namespace CardGame;

public static class EventUtility
{
	public static void GiveMoney( int amount )
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}
		
		player.Money += amount;
	}
}
