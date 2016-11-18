using UnityEngine;
using System.Collections;

public class MinionTuneSpawn : MonoBehaviour, Spawnable {

	public PlayerID owner;
	public bool copy;
    private bool crit;

    /// <summary> The tune that spawned this object. </summary>
    private Tune _tune;
    /// <summary> The tune that spawned this object. </summary>
    public Tune tune {
        get { return _tune; }
        set { _tune = value; }
    }

	public void Crit (bool crit)
	{
		this.crit = crit;
	}

	public void Owner (PlayerID owner)
	{
		this.owner = owner;

        // Switch the minion's robe color to the color of its owner.
        Transform[] robes = new Transform[2];
        Transform bardChild = transform.FindChild("bardmage_export");
        robes[0] = bardChild.FindChild("pCube2");
        robes[1] = bardChild.FindChild("pCube3");

        Material robeMaterial = LevelManager.instance.playerDict[owner].GetRobeMaterial();
        foreach (Transform robe in robes) {
            robe.GetComponent<Renderer>().material = robeMaterial;
        }
	}
		

	// Use this for initialization
	void Start () {

		if (crit) {
			Destroy (this.gameObject, 10f);
			if (!copy) {
				GameObject temp = (GameObject)GameObject.Instantiate (this.gameObject, transform.position + Vector3.right, transform.rotation);
				temp.GetComponent<MinionTuneSpawn> ().copy = true;
			}	
		} else {
			Destroy (this.gameObject, 10f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!(LevelManager.instance.playerDict [owner].GetComponent<PlayerLife> ().Alive)) {
			Destroy (this.gameObject);
		}
	}
}
