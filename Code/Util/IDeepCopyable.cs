namespace CardGame;

/// <summary>
/// Represents an object that can create a deep copy of itself.
/// </summary>
/// <typeparam name="T">The type returned by the deep copy.</typeparam>
public interface IDeepCopyable<out T>
{
	/// <summary>
	/// Creates a deep copy of the object.
	/// </summary>
	/// <returns>A new instance that is a deep copy of this object.</returns>
	T DeepCopy();
}
