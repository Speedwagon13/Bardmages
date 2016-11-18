using UnityEngine;
using System.Collections;

public class Heal : MonoBehaviour, Spawnable {
    /// <summary>
	/// The damage this attack will deal on impact
	/// This is normalized (0-1)
	/// </summary>
	public float damage;

    /// <summary>
    /// Should this be destroyed on impact?
    /// </summary>
    //public bool destroyOnImpact;

    /// <summary>
    /// How much time before this destroys itself?
    /// Use 0 for infinite.
    /// </summary>
    public float destroyAfterTime;

    public PlayerID agressor;

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

	protected virtual void Start ()
	{

        if (destroyAfterTime > 0)
        {
            Destroy(this.gameObject, destroyAfterTime);
        }
    }

    public void Crit(bool value)
    {
        crit = value;
    }

    public void Owner(PlayerID owner)
    {
        
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        BaseControl control = other.transform.root.GetComponent<BaseControl>();
        if (control && control.player == agressor)
        {
            other.transform.root.GetComponent<PlayerLife>().DealDamage(damage, tune, agressor);
            //if (destroyOnImpact) Destroy(this.gameObject);
        }
        //else if (!other.transform.root.GetComponent<PlayerControl>())
        //{
        //    if (other.GetComponent<FollowTarget>() && other.GetComponent<FollowTarget>().target.GetComponent<PlayerControl>()
        //        && other.GetComponent<FollowTarget>().target.GetComponent<PlayerControl>().player == agressor)
        //    {
        //        //TODO reorganize this section...
        //    }
        //    else
        //    {
        //        if (destroyOnImpact)
        //        {
        //            Destroy(this.gameObject);
        //            impacted = true;
        //        }
        //    }
        //}
    }
}
