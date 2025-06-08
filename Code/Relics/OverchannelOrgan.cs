namespace CardGame.Relics;

public class OverchannelOrgan( Data.Relic data ) : Relic( data )
{
	private const int MaxMpIncrease = 2;

	public override void OnAdd()
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		if ( player.Unit is null )
		{
			return;
		}

		player.Unit.MaxMp += MaxMpIncrease;
		player.Unit.Mp = player.Unit.MaxMp;
		base.OnAdd();
	}

	public override void Destroy()
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			base.Destroy();
			return;
		}

		if ( player.Unit is null )
		{
			base.Destroy();
			return;
		}

		player.Unit.MaxMp -= MaxMpIncrease;
		player.Unit.Mp = player.Unit.MaxMp;
		base.Destroy();
	}

	public override void OnCombatStart()
	{
		if ( !Owner.IsValid() )
		{
			return;
		}

		var unspent = Owner.Mana;
		var damage = unspent / 2;

		Owner.HealthComponent?.TakeFixedDamage( damage );
		base.OnCombatStart();
	}
}
