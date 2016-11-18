using UnityEngine;
using System.Collections;

/// <summary>
/// Spawns visual effects when certain events occur.
/// </summary>
public class EffectManager : MonoBehaviour {

	public static EffectManager instance;

	[SerializeField]
	private GameObject deathEffect;

	// Use this for initialization
	void Start () {
		instance = this;
	}

    /// <summary>
    /// Spawns the death effect when someone dies.
    /// </summary>
    /// <param name="position">The position to spawn the death effect at.</param>
    /// <param name="color">The color of the death effect.</param>
    public void SpawnDeathEffect(Vector3 position, Color color) {
        GameObject deathEffectInstance = GameObject.Instantiate(deathEffect,position,Quaternion.identity) as GameObject;
        deathEffectInstance.transform.FindChild("skull").GetComponent<SpriteRenderer>().color = color;
	}

	public void SpawnRespawnEffect() {

	}
}
