using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCar : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D RB;
    [SerializeField]
    private Animator animator;

    private void LateUpdate()
    {
        float xVelocity = RB.velocity.x;
        if (Mathf.Abs(xVelocity) > 0.01)
        {
            animator.SetBool("isMoving", true);
            animator.SetFloat("velocity", xVelocity);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }
}
