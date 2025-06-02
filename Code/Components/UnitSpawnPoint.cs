namespace CardGame;

public class UnitSpawnPoint : Component
{
	[Property] 
	public Faction? Faction { get; set; }
	
	[Property, ReadOnly] 
	public bool IsOccupied { get; private set; }
	
	[Property] 
	public int Order { get; set; }
	
	public void Place( GameObject gameObject, bool occupied = true )
	{		
		IsOccupied = occupied;
		gameObject.WorldTransform = WorldTransform;
	}
	
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
