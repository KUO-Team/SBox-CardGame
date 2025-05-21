using Sandbox.Diagnostics;
using CardGame.Data;

namespace CardGame;

public sealed class RelicManager : Singleton<RelicManager>
{
	[Property, WideMode, InlineEditor]
	public List<Relics.Relic> Relics { get; set; } = [];
	
	private static readonly Logger Log = new( "RelicManager" );

	protected override void OnStart()
	{
		if ( BattleManager.Instance.IsValid() )
		{
			BattleManager.Instance.OnBattleStart += OnBattleStart;
			BattleManager.Instance.OnBattleEnd += OnBattleEnd;
		}
		
		base.OnStart();
	}

	protected override void OnDisabled()
	{
		if ( BattleManager.Instance.IsValid() )
		{
			BattleManager.Instance.OnBattleStart -= OnBattleStart;
			BattleManager.Instance.OnBattleEnd -= OnBattleEnd;
		}
		
		base.OnDisabled();
	}

	public void AddRelic( Relic data )
	{
		if ( string.IsNullOrEmpty( data.Script ) )
		{
			Log.Warning( $"Can't add relic: {data.Id}; relic requires a script!" );
			return;
		}
		
		var relic = TypeLibrary.Create<Relics.Relic>( data.Script, [data] );
		Relics.Add( relic );
		relic.OnAdd();
	}

	public void RemoveRelic( Relics.Relic relic )
	{
		Relics.Remove( relic );
	}

	private void OnBattleStart( Battle battle )
	{
		if ( !Player.Local.IsValid() )
		{
			Log.Warning( "No local player found!" );
			return;
		}
		
		var playerUnit = Player.Local.Units.FirstOrDefault();
		if ( playerUnit.IsValid() )
		{
			foreach ( var relic in Relics )
			{
				relic.Owner = playerUnit;
				Log.Info( $"Set relic owner as: {playerUnit}" );
			}
		}
		else
		{
			Log.Warning( "No local player unit found!" );
		}
	}

	private static void OnBattleEnd( Battle battle )
	{
		if ( !Player.Local.IsValid() )
		{
			Log.Warning( "No local player found!" );
			return;
		}
		
		Player.Local.Units.Clear();
	}
}
