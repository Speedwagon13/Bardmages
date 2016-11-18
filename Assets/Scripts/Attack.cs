using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;

/// <summary>
/// A spawned object from an attacking tune.
/// </summary>
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

    /// <summary> The tune that spawned this object. </summary>
    private Tune _tune;
    /// <summary> The tune that spawned this object. </summary>
    public Tune tune {
        get { return _tune; }
        set { _tune = value; }
    }

    /// <summary> The time that the attack was spawned at. </summary>
    private float startTime;
    /// <summary> The amount of time that the object was active for. </summary>
    public float timeActive {
        get { return Time.time - startTime; }
    }

    /// <summary>
    /// Sets the time that the attack was spawned at.
    /// </summary>
	protected virtual void Start() {
		if(destroyAfterTime > 0) {
			Destroy(this.gameObject, destroyAfterTime);
		}
        startTime = Time.time;
	}

	public virtual void Crit(bool value) {
		crit = value;
	}

    /// <summary>
    /// Sets the owner of the object for handling who killed who
    /// </summary>
    /// <param name="owner">Owner.</param>
	public void Owner(PlayerID owner) {
        agressor = owner;
	}

    /// <summary>
    /// Damages a player on collision.
    /// </summary>
    /// <param name="other">The collider that was hit.</param>
	protected virtual void OnTriggerEnter(Collider other) {
		PlayerID player = PlayerID.None;
        BaseControl control = other.transform.GetComponent<BaseControl> ();
		if (control) {
            player = control.playerOwner;
		} else return;
		if(impacted) return;
		if (player != agressor) {
            other.transform.GetComponent<PlayerLife>().DealDamage(damage, tune, agressor);
			if(destroyOnImpact) Destroy(this.gameObject);
        } else if (!control) {
            if(player != agressor && destroyOnImpact) {
				Destroy(this.gameObject);
				impacted = true;
			}
		}
	}

}
