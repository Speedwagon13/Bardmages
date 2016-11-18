using UnityEngine;
using System.Collections;

public class Tune_Attack : Tune {

	/// <summary>
	/// What should happen when the tune completes?
	/// This implementation spawns an object, assigns an agressor, and passes in
	/// whether or not it was critical
	/// </summary>
	/// <param name="crit">Was the tune played perfectly?</param>
	public override void TuneComplete (bool crit)
	{
		base.TuneComplete (crit);

        GameObject temp = (GameObject)GameObject.Instantiate(spawnObject, GetSpawnPosition(), ownerTransform.rotation);
        Attack attack = temp.GetComponent<Attack>();
		attack.agressor = ownerTransform.GetComponent<BaseControl>().playerOwner;
        attack.tune = this;
		temp.GetComponent<Spawnable>().Crit(crit);
	}

    /// <summary>
    /// Gets the position where the tune object will spawn at.
    /// </summary>
    /// <returns>The position where the tune object will spawn at.</returns>
    protected virtual Vector3 GetSpawnPosition ()
    {
        return ownerTransform.position + ownerTransform.forward * 2f;
    }
}
