namespace CardGame.Events;

public class Masks( Data.Event data ) : Event( data )
{
	private const int MaskOfJoyId = 101;
	private const int MaskOfSorrowId = 102;
	
	public override void OnChoiceSelected( Data.Event.Choice choice, int index )
	{
		switch ( index )
		{
			case 0:
				{
					EventUtility.AddRelic( MaskOfJoyId );
					break;
				}
			case 1:
				{
					EventUtility.AddRelic( MaskOfSorrowId );
					break;
				}
			case 2:
				break;
		}
		
		base.OnChoiceSelected( choice, index );
	}
}
