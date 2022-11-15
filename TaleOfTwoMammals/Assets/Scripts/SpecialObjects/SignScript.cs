using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignScript : MonoBehaviour
{
    [SerializeField]
    private LayerMask activator;
    [SerializeField]
    private Canvas canvas;


    private void OnTriggerEnter2D(Collider2D other)
    {

        //Checks if the other layer is within collision layers
        //works by shifting the collisionLayers layermask the other.layer amount of bits to the right
        //  and checks if the bit corresponding to that layer is enabled.
        if ((activator >> other.gameObject.layer) % 2 == 1)
        {

            canvas.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((activator >> other.gameObject.layer) % 2 == 1)
        {

            canvas.gameObject.SetActive(false);
       
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
