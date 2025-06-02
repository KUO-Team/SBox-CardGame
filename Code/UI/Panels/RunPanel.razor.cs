using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI;

public partial class RunPanel
{
	[Parameter]
	public RunData? Data { get; set; }
	
	public void LoadRun()
	{
		if ( Data is null )
		{
			return;
		}
		
		if ( !Data.Version.Equals( GameInfo.Version ) )
		{
			WarningPanel? warning = null;
			warning = WarningPanel.Create( "Version Mismatch", "This run was saved on a separate game version. It might be corrupted. Are you sure you wish to load it?", [
				new Button( "Yes", "", () =>
				{
					SaveManager.Instance?.Load( Data );
					warning?.Delete();
				} ),
				new Button( "No", "", () =>
				{
					warning?.Delete();
				} )
			] );
		}
		else
		{
			SaveManager.Instance?.Load( Data );
		}
	}
	
	public void DeleteData()
	{
		if ( Data is null )
		{
			return;
		}
		
		WarningPanel? warning = null;
		warning = WarningPanel.Create( "Delete Run Data", "This action will delete the provided run data. Are you sure?", [
			new Button( "Yes", "", () =>
			{
				SaveManager.Delete( Data.Index );
				warning?.Delete();
			} ),
			new Button( "No", "", () =>
			{
				warning?.Delete();
			} )
		] );
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( Data, Data?.Date );
	}
}
