using System.Collections;

namespace CardGame;

/// <summary>
/// Allows treating a <see cref="Component"/> as a list.
/// </summary>
[Icon( "format_list_bulleted" )]
public abstract class ListComponent<T> : Component, IList<T>
{
	[Property, InlineEditor, WideMode]
	public virtual List<T> Items { get; set; } = [];

	public int Count => Items.Count;

	public bool IsReadOnly => false;

	public T this[ int index ]
	{
		get => Items[index];
		set => Items[index] = value;
	}

	public virtual void Add( T item ) => Items.Add( item );

	public virtual bool Remove( T item ) => Items.Remove( item );

	public virtual void Clear() => Items.Clear();

	public virtual bool Contains( T item ) => Items.Contains( item );

	public virtual void CopyTo( T[] array, int arrayIndex ) => Items.CopyTo( array, arrayIndex );

	public virtual int IndexOf( T item ) => Items.IndexOf( item );

	public virtual void Insert( int index, T item ) => Items.Insert( index, item );

	public virtual void RemoveAt( int index ) => Items.RemoveAt( index );

	public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
