namespace CardGame.Units;

public class CardSlotList : OwnableListComponent<CardSlot>
{
	[Property, Category( "Prefabs" )] public GameObject? TargetingArrowPrefab { get; set; }

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
			
			if ( TargetingArrowPrefab.IsValid() )
			{
				var arrow = TargetingArrowPrefab.Clone();
				slot.LineRenderer = arrow.GetComponent<LineRenderer>();
				slot.LineRenderer.UseVectorPoints = true;
			}
			
			Add( slot );
		}
	}
}
