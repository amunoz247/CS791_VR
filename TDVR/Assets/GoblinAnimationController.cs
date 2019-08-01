using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAnimationController : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator.Play("idle1");
    }
    public void walkAnim()
    {
            animator.Play("Walk");
    }
    public void attackAnim()
    {
        animator.Play("Attack1");

    }

}
