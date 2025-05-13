namespace CardGame.Effects;

public class SingletonDraw( Card card ) : CardEffect( card )
{
	public override string Description => Power.Value > 0 ? "If singleton; draw @ cards" : "If singleton; draw 1 card";
}
