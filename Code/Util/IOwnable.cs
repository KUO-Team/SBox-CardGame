using CardGame.Units;

namespace CardGame;

public interface IOwnable
{
	public BattleUnitComponent? Owner { get; set; }
}
