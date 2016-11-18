using UnityEngine;
using System.Collections;

public class CameraRaceMovement : MonoBehaviour {

	public Transform[] targets;
    public Transform[] goals;
    public float cameraSpeed = 2f;

    public int currentGoal = 0; //public so you can set the starting goal

	private Vector3 offset;

	private Vector3 lookPosition;

	private float initialFOV;


	// Use this for initialization
	void Start () {
		offset = transform.position;

        initialFOV = GetComponent<Camera>().fieldOfView;
		lookPosition = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 leadPosition = targets[GetLeader()].position;

        GetComponent<Camera>().fieldOfView = LevelManager.instance.BeatValue(0f)/2f + initialFOV;
		transform.GetChild(0).GetComponent<Camera>().fieldOfView = LevelManager.instance.BeatValue(0f)/2f + initialFOV;

		transform.localPosition = Vector3.MoveTowards(transform.localPosition,leadPosition + offset, Time.deltaTime*Vector3.Distance(transform.localPosition,leadPosition + offset)*cameraSpeed);
		//lookPosition = Vector3.MoveTowards(lookPosition,leadPosition,Time.deltaTime*Vector3.Distance(lookPosition,leadPosition));
		//transform.LookAt(lookPosition);

	}

    int GetLeader() {
        int currentLeader = 0;
        int previousGoal = currentGoal - 1;
        if (previousGoal < 0) previousGoal = goals.Length - 1;
        Vector3 raceDirection = (goals[currentGoal].position - goals[previousGoal].position).normalized;

        //checking which of the players reached the farthest position
        float maxTravelledDistance = -100000; //towards current goal
        for(int i = 0; i<targets.Length; i++) { 
            float travelled = Vector3.Dot(targets[i].position - goals[previousGoal].position, raceDirection);
            if (travelled > maxTravelledDistance) {
                currentLeader = i;
                maxTravelledDistance = travelled;
            }
        }
        currentLeader = 0;
        //changing the goal to next one if necessary
        if (maxTravelledDistance > (goals[currentGoal].position - goals[previousGoal].position).magnitude)
            currentGoal = currentGoal == goals.Length - 1 ? 0 : currentGoal + 1;
        return currentLeader;
    }
}
