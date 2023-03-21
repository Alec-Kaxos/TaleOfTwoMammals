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

    [Header("Button Settings")]
    [SerializeField, Tooltip("Set if button should never be released after being pressed.")]
    private bool StaysPressed = false;

    private BoxCollider2D boxCollider;
    [SerializeField]
    private LayerMask collisionLayers;

    [SerializeField]
    private string TongueTag;

    [Header("Button Sprite Settings")]
    [SerializeField]
    private SpriteRenderer SpriteR;
    [SerializeField]
    private Sprite ButtonNormal;
    [SerializeField]
    private Sprite ButtonPushed;
    [SerializeField]
    private AudioSource ButtonClickSound;


    #region Press/Release

    /// <summary>
    /// Use when the button is initially pressed. Calls onPress action.
    /// </summary>
    protected void PressButton()
    {
        if (!isPressed)
        {
            PressSpecifics();

            isPressed = true;
            onPress();
            ButtonClickSound.Play();

            //Debug.Log("Pressed " + gameObject);
        }
    }

    /// <summary>
    /// Use when the button is unpressed (all objects leave button). Calls the onRelase action.
    /// </summary>
    protected void ReleaseButton()
    {
        if (!StaysPressed && isPressed)
        {
            ReleaseSpecifics();

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


    /// <summary>
    /// Specific functions to do to this button object when it is pressed.
    /// </summary>
    protected virtual void PressSpecifics()
    {
        //Currently changes the sprite
        SpriteR.sprite = ButtonPushed;
    }
    
    /// <summary>
    /// Specific functions to do to this button object when it is released.
    /// </summary>
    protected virtual void ReleaseSpecifics()
    {
        //Currently changes the sprite
        SpriteR.sprite = ButtonNormal;
    }

    #endregion

    #region Unity Actions
    // Start is called before the first frame update
    void Start()
    {

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
        if (((collisionLayers >> other.gameObject.layer) % 2 == 1) || other.gameObject.tag == TongueTag)
        {
            overlapping++;

            //One(first) object has now entered
            if (overlapping == 1)
            {
                PressButton();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Duplicate of above, but subtracts one on exit
        if (((collisionLayers >> other.gameObject.layer) % 2 == 1) || other.gameObject.tag == TongueTag)
        {
            overlapping--;

            //Now all objects are off
            if (overlapping == 0)
            {
                ReleaseButton();
            }
        }
    }

    #endregion (Start/Update)
}
