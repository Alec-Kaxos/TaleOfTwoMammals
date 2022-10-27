using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{

    //All the GameObjects that this button will call both pressed() and unpressed() on
    private List<ButtonSubscriber> subscribers;
    private bool isPressed;

    #region Press/Unpress Subscription

    /// <summary>
    /// Subscribe to recieve pressed and unpressed updates from this button.
    /// </summary>
    /// <param name="s">The ButtonSubscriber object in question.</param>
    public void Subscribe(ButtonSubscriber s)
    {
        subscribers.Add(s);
    }

    /// <summary>
    /// Use when the button is initially pressed. Calls the pressed function on all ButtonSubscribers.
    /// </summary>
    protected void Press()
    {
        isPressed = true;
        foreach(ButtonSubscriber s in subscribers)
        {
            s.Pressed(gameObject);
        }
    }

    /// <summary>
    /// Use when the button is unpressed. Calls the unpressed function on all ButtonSubscribers.
    /// </summary>
    protected void Unpress()
    {
        isPressed = false;
        foreach (ButtonSubscriber s in subscribers)
        {
            s.Unpressed(gameObject);
        }
    }
    
    /// <returns>Whether the button is pressed or not</returns>
    public bool Pressed()
    {
        return isPressed;
    }

    #endregion

    #region Unity Actions
    // Start is called before the first frame update
    void Start()
    {
        subscribers = new List<ButtonSubscriber>();
        isPressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion (Start/Update)
}
