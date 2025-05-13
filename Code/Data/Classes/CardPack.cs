namespace CardGame.Data;

public class CardPack : IResource, IDeepCopyable<CardPack>
{
	[InlineEditor]
	public Id Id { get; set; } = Id.Invalid;

	public string Name { get; set; } = "";

	public CardPackRarity Rarity { get; set; }

	[InlineEditor, WideMode]
	public List<Id> Cards { get; set; } = [];

	public List<Card> Open( int amount = 3 )
	{
		List<Card> cards = [];

		for ( var i = 0; i < amount; i++ )
		{
			var cardId = Game.Random.FromList( Cards! );
			if ( cardId is not null && CardDataList.GetById( cardId ) is {} card )
			{
				cards.Add( card );
			}
		}
		return cards;
	}
	
	/// <summary>
	/// Creates a deep copy of this card pack.
	/// </summary>
	public CardPack DeepCopy()
	{
		var card = new CardPack()
		{
			Id = Id,
			Name = Name,
			Rarity = Rarity,
			Cards = [..Cards],
		};

		return card;
	}
	
	public override string ToString()
	{
		return $"CardPack: {Name} - Id: {Id.LocalId}";
	}
	
	public enum CardPackRarity
	{
		Common,
		Uncommon,
		Rare,
		Epic
	}
}
