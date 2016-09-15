using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;

public class Attack : MonoBehaviour, Spawnable {
	/// <summary>
	/// The damage this attack will deal on impact
	/// This is normalized (0-1)
	/// </summary>
	public float damage;

	/// <summary>
	/// Should this be destroyed on impact?
	/// </summary>
	public bool destroyOnImpact;

	/// <summary>
	/// How much time before this destroys itself?
	/// Use 0 for infinite.
	/// </summary>
	public float destroyAfterTime;

	public PlayerID agressor, victim;

	[HideInInspector]
	public bool impacted;

	protected bool crit;

	protected virtual void Start() {
		if(destroyAfterTime > 0) {
			Destroy(this.gameObject, destroyAfterTime);
		}
	}

	public void Crit(bool value) {
		crit = value;
	}

	public void Owner(PlayerID owner) {

	}

    /// <summary>
    /// Damages a player on collision.
    /// </summary>
    /// <param name="other">The collider that was hit.</param>
	protected virtual void OnTriggerEnter(Collider other) {
		if(impacted) return;
		if (other.transform.root.GetComponent<BaseControl>()
			&& other.transform.root.GetComponent<BaseControl>().player != agressor) {
			other.transform.root.GetComponent<PlayerLife>().DealDamage(damage);
			if(destroyOnImpact) Destroy(this.gameObject);
		} else if (!other.transform.root.GetComponent<BaseControl>()) {
			if (other.GetComponent<FollowTarget>() && other.GetComponent<FollowTarget>().target.GetComponent<BaseControl>()
				&& other.GetComponent<FollowTarget>().target.GetComponent<BaseControl>().player == agressor) {
				//TODO reorganize this section...
			} else {
				if(destroyOnImpact) {
					Destroy(this.gameObject);
					impacted = true;
				}
			}
		}
	}

}
