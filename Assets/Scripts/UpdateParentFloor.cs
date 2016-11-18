using UnityEngine;
using System.Collections;

/// <summary>
/// Keeps track of the part of the floor that the object is standing on.
/// </summary>
public class UpdateParentFloor : MonoBehaviour {

    /// <summary> All floor objects in the scene. </summary>
	private GameObject[] allFloors;

    /// <summary>
    /// Finds all floors in the scene.
    /// </summary>
    private void Start() {
        if (allFloors == null) {
            allFloors = GameObject.FindGameObjectsWithTag("MovingFloor");
        }
    }

    /// <summary>
    /// Keeps track of the part of the floor that the object is standing on.
    /// </summary>
    protected void Update() {
		if(allFloors.Length > 0) {
	        GameObject closestFloor = null;
	        float nearestDistanceSquared = Mathf.Infinity;
	        foreach (GameObject floor in allFloors) {
				if(floor == null) continue;
	            var floorPosition = floor.transform.position;
	            var currDistanceSquared = (floorPosition - transform.position).sqrMagnitude;
	            if (currDistanceSquared < nearestDistanceSquared) {
	                closestFloor = floor;
	                nearestDistanceSquared = currDistanceSquared;
	            }
	        }

	        if (closestFloor != null) {
	            transform.parent = closestFloor.transform;
	        }
		}
    }
}
