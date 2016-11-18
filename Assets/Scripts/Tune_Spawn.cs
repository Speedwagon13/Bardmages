using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;

public class Tune_Spawn : Tune {

	/// <summary>
	/// Should the spawned object be attached to the player
	/// Requires a FollowTarget script to be attached to the spawn object
	/// </summary>
	public bool attach;

    /// <summary>
    /// What should happen when the tune completes?
    /// </summary>
    /// <param name="crit">Was the tune played perfectly?</param>
	public override void TuneComplete (bool crit)
	{
		base.TuneComplete (crit);
		GameObject temp = (GameObject)GameObject.Instantiate(spawnObject, ownerTransform.position, ownerTransform.rotation);
		if(attach) {
			temp.AddComponent<FollowTarget>();
			temp.GetComponent<FollowTarget>().target = ownerTransform;
			temp.GetComponent<FollowTarget>().offset = Vector3.zero;
		}
        Spawnable spawnable = temp.GetComponent<Spawnable>();
        if(spawnable != null) {
            PlayerID ownerID = ownerTransform.GetComponent<BaseControl>().playerOwner;
            spawnable.Owner(ownerID);
            spawnable.Crit(crit);
            if (ownerTransform.GetComponent<MinionTuneSpawn>() != null) {
                BaseBard bard = LevelManager.instance.GetBardFromID(ownerID);
                spawnable.tune = bard.GetTuneFromName("Minion");
            }
            if (spawnable.tune == null) {
                spawnable.tune = this;
            }
		}
	}
}
