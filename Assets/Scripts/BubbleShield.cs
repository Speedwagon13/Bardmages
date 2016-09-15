using UnityEngine;
using System.Collections;

public class BubbleShield : MonoBehaviour, Spawnable {

	[HideInInspector]
	public int numBlocks;

	[HideInInspector]
	public PlayerID owner;

	void Start() {
		Destroy(this.gameObject,1f);
	}

	void OnTriggerEnter(Collider other) {
		if(other.GetComponent<Attack>()) numBlocks--;
		if(numBlocks <= 0) Destroy(this.gameObject);
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
				if(c.transform.root.GetComponent<BaseControl>()
                    && c.transform.root.GetComponent<BaseControl>().player != owner) {
                    c.transform.root.GetComponent<BaseControl>().Knockback(c.transform.root.position - transform.position);
				}
			}
		}
	}

	public void Owner(PlayerID owner) {
		this.owner = owner;
	}
}
