#nullable enable

using System.Collections.Generic;
using Sandbox;

namespace kEllie.Utils;

public abstract class PanelSingleton<T> : PanelComponent, IHotloadManaged where T : PanelSingleton<T>
{
	public static T? Instance { get; private set; }

	protected override void OnAwake()
	{
		if ( Instance.IsValid() )
		{
			Log.Warning( $"Singleton tried to initialize another {typeof(T)}!" );
			Destroy();
			return;
		}

		if ( Active )
		{
			Instance = (T)this;
		}
	}

	void IHotloadManaged.Destroyed( Dictionary<string, object> state )
	{
		state["IsActive"] = Instance == this;
	}

	void IHotloadManaged.Created( IReadOnlyDictionary<string, object> state )
	{
		if ( state.GetValueOrDefault( "IsActive" ) is true )
		{
			Instance = (T)this;
		}
	}

	protected override void OnDestroy()
	{
		if ( Instance == this )
		{
			Instance = null;
		}
	}
}
