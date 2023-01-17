using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Snake : MonoBehaviour
{
    [SerializeField]
    private float delay = 1.0f;
    [SerializeField]
    private Animator animator;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //sets up a timer for the snake to appear at a constant interval
        timer += Time.deltaTime;
        if(timer > delay)
        {
            animator.SetBool("Active", true);
            //The 2.5f accounts for the time it takes for the animation to finish
            timer -= delay + 2.5f ;
        }
        else
        {
            animator.SetBool("Active", false);
        }
        
    }
}
