using System.Diagnostics.CodeAnalysis;

namespace CardGame.Data;

public abstract class DataList<T> : GameResource where T : IResource
{
	[InlineEditor, WideMode]
	public List<T> List { get; set; } = [];

	public static IReadOnlyList<T> All
	{
		get
		{
			return _cache ??= ResourceLibrary.GetAll<DataList<T>>()
				.SelectMany( x => x.List )
				.ToList();
		}
	}

	private static IReadOnlyList<T>? _cache;

	public static T? GetById( Id id )
	{
		var item = All.FirstOrDefault( x => x.Id.Equals( id ) );
		item?.OnInit();
		return item;
	}

	public static bool TryGetById( Id id, [NotNullWhen( true )] out T? result )
	{
		result = All.FirstOrDefault( x => x.Id.Equals( id ) );
		if ( result is null )
		{
			return false;
		}

		result.OnInit();
		return true;
	}

	public static void InvalidateCache()
	{
		_cache = null;
	}

	protected override void PostReload()
	{
		InvalidateCache();
		base.PostReload();
	}
}
