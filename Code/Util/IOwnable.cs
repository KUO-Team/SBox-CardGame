using CardGame.Units;

namespace CardGame;

public interface IOwnable
{
	public BattleUnit? Owner { get; set; }
}
