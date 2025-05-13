namespace CardGame;

public class UnitSpawnPoint : Component
{
	[Property] 
	public Faction? Faction { get; set; }
	
	[Property] 
	public bool IsOccupied { get; set; }
	
	[Property] 
	public int Order { get; set; }
	
	protected override void DrawGizmos()
	{
		base.DrawGizmos();
		var spawnModel = Model.Load( "models/editor/spawnpoint.vmdl" );

		if ( spawnModel is null )
		{
			return;
		}
		
		Gizmo.Hitbox.Model( spawnModel );
		var so = Gizmo.Draw.Model( spawnModel );
		if ( so is not null )
		{
			so.Flags.CastShadows = true;
		}
	}
}
