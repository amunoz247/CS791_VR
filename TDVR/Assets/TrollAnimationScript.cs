using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrollAnimationScript : MonoBehaviour
{
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator.Play("walk");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
