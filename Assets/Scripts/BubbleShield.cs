using UnityEngine;
using System.Collections;

public class BubbleShield : MonoBehaviour, Spawnable {

	// handels how many blocks this bubble can take
	[HideInInspector]
	public int numBlocks;

	[HideInInspector]
    public PlayerID owner;

    /// <summary> The tune that spawned this object. </summary>
    private Tune _tune;
    /// <summary> The tune that spawned this object. </summary>
    public Tune tune {
        get { return _tune; }
        set { _tune = value; }
    }

	void Start() {
		Destroy(this.gameObject,1f);
	}

    /// <summary>
    /// Depletes the shield after blocking an attack.
    /// </summary>
    /// <param name="damage">The damage that would have been dealt.</param>
    public void BlockAttack(float damage) {
        LevelManager.instance.GetBardFromID(owner).CreditHit(tune, damage);
        if(--numBlocks <= 0) {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Knocks other players back if used perfectly.
    /// </summary>
    /// <param name="value">Whether the shield was used perfectly.</param>
	public void Crit(bool value) {
		if(value) {
			transform.GetChild(0).gameObject.SetActive(true);
			Collider[] colliders = Physics.OverlapSphere(transform.position, 15f);
			foreach (Collider c in colliders) {
                BaseControl control = c.transform.root.GetComponent<BaseControl>();
                if(control && control.player != owner) {
					control.Knockback((c.transform.root.position - transform.position).normalized*1f);
				}
			}
		}
	}

    /// <summary>
    /// Sets the owner of the object for handling who killed who
    /// </summary>
    /// <param name="owner">Owner.</param>
	public void Owner(PlayerID owner) {
		this.owner = owner;
        transform.parent = LevelManager.instance.playerDict[owner].transform;
	}
}
