using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : ButtonUser
{
    //[SerializeField]
    //private ButtonScript[] buttons;

    //private int currentButtonsPressed;

    //private bool doorOpen = false;

    [SerializeField]
    private float squishAmt = 10;

    /* OLD Button Functionality
    #region Button Functionality

    public void Pressed()
    {
        currentButtonsPressed++;

        //Check if door should be open
        if (currentButtonsPressed == buttons.Length)
        {
            open();
        }
    }

    public void Unpressed()
    {
        currentButtonsPressed--;

        //Check if door should be closed
        if (currentButtonsPressed != buttons.Length)
        {
            close();
        }
    }

    #endregion
    */

    #region Door Logic

    /// <summary>
    /// Called when the door opens (all connected buttons pressed). Only opens if not already open.
    /// </summary>
    protected virtual void open()
    {
        //if(!doorOpen)
        //{
        transform.localPosition += new Vector3( 0, (transform.localScale.y - transform.localScale.y / squishAmt) / 2, 0 );
        transform.localScale += new Vector3(0,transform.localScale.y / squishAmt - transform.localScale.y ,0);

        //    doorOpen = true;
        //Debug.Log("Open");
        //}

    }

    /// <summary>
    /// Called when the door closes, while not already closed. Happens when one button is released while all are pressed.
    /// </summary>
    protected virtual void close()
    {
        //if (doorOpen)
        //{
        transform.localPosition += new Vector3(0, (transform.localScale.y - transform.localScale.y * squishAmt) / 2, 0);
        transform.localScale += new Vector3(0, transform.localScale.y * squishAmt - transform.localScale.y, 0);

        //    doorOpen = false;
        //Debug.Log("Close");
        //}
    }

    protected override void Activated()
    {
        open();
    }

    protected override void Deactivated()
    {
        close();
    }

    #endregion

    #region Unity Functions

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //currentButtonsPressed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    private void OnEnable()
    {
        subscribe();   
    }

    private void OnDisable()
    {
        unsubscribe();
    }

    private void subscribe()
    {
        currentButtonsPressed = 0;

        foreach (ButtonScript b in buttons)
        {
            b.onPress += Pressed;
            b.onRelease += Unpressed;
            if (b.IsPressed()) currentButtonsPressed++;
        }

        if (currentButtonsPressed == buttons.Length)
        {
            open();
        }
    }

    private void unsubscribe()
    {
        currentButtonsPressed = 0;


        foreach (ButtonScript b in buttons)
        {
            b.onPress -= Pressed;
            b.onRelease -= Unpressed;
        }

        close();

    }
    */

    #endregion
}
