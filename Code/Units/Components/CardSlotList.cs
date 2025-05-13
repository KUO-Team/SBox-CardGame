namespace CardGame.Units;

public class CardSlotList : OwnableListComponent<CardSlot>
{
	public void AddCardSlot( int amount = 1 )
	{
		if ( !Owner.IsValid() )
		{
			Log.Warning( $"Unable to add card slot; no owner!" );
			return;
		}

		for ( var i = 0; i < amount; i++ )
		{
			var slot = Owner.AddComponent<CardSlot>();
			slot.Owner = Owner;
			Add( slot );
		}
	}
}
