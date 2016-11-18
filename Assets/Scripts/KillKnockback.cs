using UnityEngine;
using System.Collections;

public class KillKnockback : MonoBehaviour {
	void Update () {
	    if (GetComponent<ParticleSystem>().isPlaying)
        {
            Destroy(this.gameObject, 1f);
        }
	}
}
