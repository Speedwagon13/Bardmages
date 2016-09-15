/// <summary>
/// Implement this interface on any object that needs to be spawned
/// so that every item spawned by a tune can know its owner and know
/// whether or not a critical play was achieved
/// </summary>
public interface Spawnable {

	/// <summary>
	/// What should this object do if a critical play was achieved?
	/// </summary>
	/// <param name="crit">Whether or not a critical play was achieved</param>
	void Crit(bool crit);

	/// <summary>
	/// Sets the owner of the object for handling who killed who
	/// </summary>
	/// <param name="owner">Owner.</param>
	void Owner(PlayerID owner);

}
