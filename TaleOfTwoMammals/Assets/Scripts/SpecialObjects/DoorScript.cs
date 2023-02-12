using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DoorScript : ButtonUser
{
    [Header("Door Options")]
    [SerializeField]
    private SpriteRenderer Renderer;
    [SerializeField]
    private Sprite OpenSprite;
    [SerializeField]
    private Sprite ClosedSprite;
    [SerializeField]
    private BoxCollider2D OpenCollider;
    [SerializeField]
    private BoxCollider2D ClosedCollider;
    //private float squishAmt = 10;

    #region Door Logic

    /// <summary>
    /// Called when the door opens (all connected buttons pressed). Only opens if not already open.
    /// </summary>
    protected virtual void open()
    {
        //Set to open sprite and collider
        Renderer.sprite = OpenSprite;
        OpenCollider.enabled = true;
        ClosedCollider.enabled = false;
        

        //Currently Squishes the door
        //transform.localPosition += new Vector3( 0, (transform.localScale.y - transform.localScale.y / squishAmt) / 2, 0 );
        //transform.localScale += new Vector3(0,transform.localScale.y / squishAmt - transform.localScale.y ,0);

    }

    /// <summary>
    /// Called when the door closes, while not already closed. Happens when one button is released while all are pressed.
    /// </summary>
    protected virtual void close()
    {
        //Set to closed sprite and collider
        Renderer.sprite = ClosedSprite;
        OpenCollider.enabled = false;
        ClosedCollider.enabled = true;
        

        //Currently Squishes the door
        //transform.localPosition += new Vector3(0, (transform.localScale.y - transform.localScale.y * squishAmt) / 2, 0);
        //transform.localScale += new Vector3(0, transform.localScale.y * squishAmt - transform.localScale.y, 0);
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
