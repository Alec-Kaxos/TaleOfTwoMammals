using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class ButtonScript : MonoBehaviour
{

    //All the GameObjects that this button will call both pressed() and unpressed() on
    public Action onPress;
    public Action onRelease;

    private bool isPressed;
    //The amount of currently overlapping objects
    private int overlapping;

    private BoxCollider2D boxCollider;
    [SerializeField]
    private LayerMask collisionLayers;

#region Press/Unpress Subscription

    /// <summary>
    /// Use when the button is initially pressed. Calls the pressed function on all ButtonSubscribers.
    /// </summary>
    protected void Press()
    {
        if (!isPressed)
        {
            transform.localScale += new Vector3(0, -transform.localScale.y / 2, 0);

            isPressed = true;
            onPress();

            //Debug.Log("Pressed " + gameObject);
        }
    }

    /// <summary>
    /// Use when the button is unpressed. Calls the unpressed function on all ButtonSubscribers.
    /// </summary>
    protected void Unpress()
    {
        if (isPressed)
        {
            transform.localScale += new Vector3(0, transform.localScale.y, 0);

            isPressed = false;
            onRelease();

            //Debug.Log("Unpressed " + gameObject);
        }
    }

    /// <returns>Whether the button is pressed or not</returns>
    public bool IsPressed()
    {
        return isPressed;
    }

    #endregion

#region Unity Actions
    // Start is called before the first frame update
    void Start()
    {

        /*
        //Set up box collider and the number of currently colliding objects (should button already be pressed?)
        boxCollider = GetComponent<BoxCollider2D>();
        // Set your filters here according to https://docs.unity3d.com/ScriptReference/ContactFilter2D.html
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.layerMask = collisionLayers;
        overlapping = boxCollider.OverlapCollider(contactFilter, new Collider2D[1]);
        */

        overlapping = 0;
        isPressed = overlapping > 0;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        //Checks if the other layer is within collision layers
        //works by shifting the collisionLayers layermask the other.layer amount of bits to the right
        //  and checks if the bit corresponding to that layer is enabled.
        if ((collisionLayers >> other.gameObject.layer) % 2 == 1)
        {
            overlapping++;

            //One(first) object has now entered
            if (overlapping == 1)
            {
                Press();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {

        //Duplicate of above, but subtracts one on exit
        if ((collisionLayers >> other.gameObject.layer) % 2 == 1)
        {
            overlapping--;

            //Now all objects are off
            if (overlapping == 0)
            {
                Unpress();
            }
        }
    }

    #endregion (Start/Update)
}
