using System;

namespace CardGame.Data;

public class RunData
{
	public string Version { get; set; } = string.Empty;

	public int Index { get; set; }

	public int Seed { get; set; }

	public int MapNodeIndex { get; set; }

	public List<int> CompletedNodes { get; set; } = [];

	public int Floor { get; set; }

	public int Money { get; set; }

	public Id Class { get; set; } = Id.Invalid;

	public UnitData UnitData { get; set; } = new();

	public List<Id> Cards { get; set; } = [];

	public List<Id> CardPacks { get; set; } = [];

	public List<Id> Relics { get; set; } = [];

	public DateTime Date { get; set; } = DateTime.Now;
}
