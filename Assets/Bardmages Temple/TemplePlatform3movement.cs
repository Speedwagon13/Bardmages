using UnityEngine;
using System.Collections;

public class TemplePlatform3movement : MonoBehaviour
{
    bool condition = true;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x >= 9.22)
            condition = false;
        if (transform.position.x <= -9.22)
            condition = true;
        if (condition == true)
            transform.Translate(Vector3.right * Time.deltaTime * 5f);
        else if (condition == false)
            transform.Translate(Vector3.left * Time.deltaTime * 5f);
    }
}