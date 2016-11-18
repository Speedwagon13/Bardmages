using UnityEngine;
using System.Collections;

public class AliceLevelBG : MonoBehaviour {

	public float rotateSpeed;
	public float startRot;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Camera.main.transform.position + Camera.main.transform.forward*500f;
		startRot += rotateSpeed * Time.deltaTime;
		transform.LookAt(Camera.main.transform.position);
		transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,startRot);
	}
}
