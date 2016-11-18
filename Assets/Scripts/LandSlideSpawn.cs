using UnityEngine;
using System.Collections;

/// <summary>
/// A large obstacle that damages and pushes players.
/// </summary>
public class LandSlideSpawn : Attack {


	private PlayerID owner;
	private bool crit;
	private float secondRockSpawn = .3f;
	private float thirdRockSpawn = .6f;
	public GameObject rock2, rock3;

	// <summary>
    // What should this object do if a critical play was achieved?
    // </summary>
    // <param name="crit">Whether or not a critical play was achieved</param>
	public override void Crit (bool crit) {
		this.crit = crit;
		if (crit) {
			transform.localScale = new Vector3 (1.2f, 1f, 1f);
		}
        base.Crit(crit);
	}



		

    /// <summary>
    /// Initializes fields depending on critical hit status.
    /// </summary>
	protected override void Start () {
        destroyAfterTime = 8f;
        damage = crit ? .4f : .2f;
        base.Start();
	}


	void Update() {
		Vector3 dwn = transform.TransformDirection(Vector3.down);
		transform.Translate (0, 0, 20f * Time.deltaTime, Space.Self);
		if (!Physics.Raycast (transform.position, dwn, 2f)) {
			Destroy (this.gameObject);
		}

		secondRockSpawn -= Time.deltaTime;
		thirdRockSpawn -= Time.deltaTime;
		if (secondRockSpawn <= 0) {
			rock2.SetActive (true);
		}
		if (thirdRockSpawn <= 0) {
			rock3.SetActive (true);
		}
	}

}