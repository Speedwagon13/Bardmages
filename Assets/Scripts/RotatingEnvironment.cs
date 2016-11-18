using UnityEngine;
using System.Collections;

public class RotatingEnvironment : MonoBehaviour {

	public Material skyBox;
	public float offset;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up*Time.deltaTime*10f, Space.World);
		skyBox.SetFloat("_Rotation", -transform.eulerAngles.y + offset);
	}
}
