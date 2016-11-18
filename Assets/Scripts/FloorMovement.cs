using UnityEngine;
using System.Collections;

public class FloorMovement : MonoBehaviour {

    private float timer;
    public float xDirection;
    public float yDirection;
    public float zDirection;
    public float xRotation;
    public float yRotation;
    public float zRotation;
    public float speed;

    void Update() {
        transform.Rotate(xRotation * Time.deltaTime, yRotation * Time.deltaTime, zRotation * Time.deltaTime);

        transform.Translate(-Vector3.right * xDirection * Time.deltaTime * speed);
        transform.Translate(-Vector3.up * yDirection * Time.deltaTime * speed);
        transform.Translate(-Vector3.forward * zDirection * Time.deltaTime * speed);

        timer -= Time.deltaTime;
        if (timer <= 0) {
            xDirection = -xDirection;
            yDirection = -yDirection;
            zDirection = -zDirection;
            timer = 2;
        }

    }
}
