using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui : MonoBehaviour
{
    private bool isRotating = false;
    private float theta = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating) {
            transform.position = transform.position + new Vector3(Mathf.Sin(theta), 0, Mathf.Cos(theta));
            theta += 0.05f;
        }
    }

    public void StartRotating() {
        Debug.Log("This button was clicked!");
    }
}
