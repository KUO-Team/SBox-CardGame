using System;
using System.Text.Json.Serialization;

namespace CardGame.Data;

public class CardPack : IResource, IDeepCopyable<CardPack>
{
	[InlineEditor]
	public Id Id { get; set; } = Id.Invalid;

	public string Name { get; set; } = "";

	public CardPackRarity Rarity { get; set; }

	public CardPackAvailabilities Availabilities { get; set; } = CardPackAvailabilities.None;

	public TagSet Keywords { get; set; } = new();

	[InlineEditor, WideMode]
	public List<Id> Cards { get; set; } = [];
	
	[Hide, JsonIgnore]
	public bool IsAvailable
	{
		get
		{
			if ( Availabilities.HasFlag( CardPack.CardPackAvailabilities.Starter ) )
			{
				return true;
			}

			if ( Availabilities.HasFlag( CardPack.CardPackAvailabilities.Shop ) )
			{
				return true;
			}

			if ( Availabilities.HasFlag( CardPack.CardPackAvailabilities.Event ) )
			{
				return true;
			}

			return false;
		}
	}

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
	
	[Flags]
	public enum CardPackAvailabilities
	{
		None = 0,
		[Description( "Found in shops" )]
		Shop = 1 << 0,
		[Description( "In starting deck" )]
		Starter = 1 << 1,
		[Description( "Given as a reward" )]
		Reward = 1 << 2,
		[Description( "Found in chests" )]
		Chest = 1 << 3,
		[Description( "Granted in special events" )]
		Event = 1 << 4,
		[Description( "Only available in dev builds/testing" )]
		DevOnly = 1 << 5,
	}
}
