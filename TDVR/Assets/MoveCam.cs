using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    public float Speed = 5.0f;

    void Update()
    {
        if (Input.GetKey(KeyCode.D)) // Move Right
        {
            transform.position = new Vector3(transform.position.x + Speed, transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.A)) // Move Left
        {
            transform.position = new Vector3(transform.position.x - Speed, transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.S)) // Move Backward
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - Speed);
        }
        if (Input.GetKey(KeyCode.W)) // Move Forward
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Speed);
        }
    }
}
