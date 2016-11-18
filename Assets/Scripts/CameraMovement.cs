using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public Transform[] targets;

	private Vector3 offset;

	private Vector3 lookPosition;

	private float initialFOV;

	private float startingSlow;

	// Use this for initialization
	void Start () {
		offset = new Vector3(0f,60f,-30f);
		initialFOV = GetComponent<Camera>().fieldOfView;
		lookPosition = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		if(startingSlow < 1f) startingSlow += Time.deltaTime/4f;

		Vector3 averagePosition = Vector3.zero;

		int numDead = 0;
		float maxDistance = 0f;

		for(int i = 0; i < 4; i++) {
			for(int j = 0; j < 4; j++) {
				if(targets[i].GetComponent<PlayerLife>().Alive && targets[j].GetComponent<PlayerLife>().Alive) {
					if(Vector3.Distance(targets[i].position,targets[j].position) > maxDistance) {
						maxDistance = Vector3.Distance(targets[i].position,targets[j].position);
					}
				}
			}
		}

		foreach(Transform t in targets) {
            if(t.gameObject.activeSelf && t.GetComponent<PlayerLife>().Alive) {
				averagePosition += t.position;
			} else {
				numDead++;
			}
		}

        if (targets.Length <= numDead) {
            return;
        }

		averagePosition /= (targets.Length - numDead);

		GetComponent<Camera>().fieldOfView = LevelManager.instance.BeatValue(0f)/2f + initialFOV;
		transform.GetChild(0).GetComponent<Camera>().fieldOfView = LevelManager.instance.BeatValue(0f)/2f + initialFOV;

		transform.localPosition = Vector3.MoveTowards(transform.localPosition,averagePosition + offset*(maxDistance/35f), Time.deltaTime*Vector3.Distance(transform.localPosition,averagePosition + offset*(maxDistance/35f))/2f);
		lookPosition = Vector3.MoveTowards(lookPosition,averagePosition,Time.deltaTime*Vector3.Distance(lookPosition,averagePosition));
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookPosition - transform.position), Time.deltaTime*Quaternion.Angle(transform.rotation, Quaternion.LookRotation(lookPosition - transform.position))*startingSlow);

	}
}
