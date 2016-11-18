using UnityEngine;
using System.Collections;

public class Land_Spawn : MonoBehaviour, Spawnable {

	private PlayerID owner;
	private bool crit;
	public GameObject spawnObject;
	private GameObject temp;
	private LandSlideSpawn temporary;


	void Start () {
		temp = (GameObject)Instantiate (spawnObject, transform.position + transform.forward * 3, transform.rotation);
		Destroy (transform.root.gameObject, 4f);
		temporary = temp.GetComponent<LandSlideSpawn>();
		temporary.Crit (crit);
		temporary.Owner (owner);
	}

	void OnTriggerEnter(Collider other) {

	}

	void Update() {
		
	}

	public void Crit (bool crit) {
		this.crit = crit;
	}

	public void Owner (PlayerID owner) {
		this.owner = owner;
	}

	// <summary> The tune that spawned this object. </summary>
	private Tune _tune;
	// <summary> The tune that spawned this object. </summary>
	public Tune tune {
		get { return _tune; }
		set { _tune = value; }
	}
}

