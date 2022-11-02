using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ButtonUser : MonoBehaviour
{
    [SerializeField, Tooltip("All the buttons this ButtonUser will link to.")]
    private ButtonScript[] Buttons;
    [SerializeField, Tooltip("The minimum number of buttons needed to be pressed to activate (and deactivate) this ButtonUser. Set to -1 for all buttons to be required."), Min(-1)]
    private int ButtonsRequired = -1;
    private int CurrentPressed;

    private bool NowActivated;


    #region Unity Functions

    // Start is called before the first frame update
    protected virtual void Start()
    {

    }

    protected virtual void OnEnable()
    {
        //If the buttons required is -1 (all buttons), set to the length of the Buttons[] array.
        if (ButtonsRequired == -1)
        {
            ButtonsRequired = Buttons.Length;
        }

        NowActivated = false;

        Subscribe();
    }

    protected virtual void OnDisable()
    {
        Unsubscribe();
    }


    #endregion

    #region Subscription
    private void Subscribe()
    {
        CurrentPressed = 0;

        foreach (ButtonScript b in Buttons)
        {
            b.onPress += Pressed;
            b.onRelease += Released;
            if (b.IsPressed()) CurrentPressed++;
        }

        if (CurrentPressed >= ButtonsRequired)
        {
            InternalActivate();
        }
    }

    private void Unsubscribe()
    {
        CurrentPressed = 0;

        foreach (ButtonScript b in Buttons)
        {
            b.onPress -= Pressed;
            b.onRelease -= Released;
        }

        InternalDeactivate();

    }

    #endregion

    #region Press Release Logic

    /// <summary>
    /// Called when the required amount of buttons has been pressed. Alternates being called with Deactivated().
    /// </summary>
    protected abstract void Activated();

    /// <summary>
    /// Called when enough buttons have been released (while also in an active state). Alternates being called with Activated().
    /// </summary>
    protected abstract void Deactivated();

    public bool IsActivated()
    {
        return NowActivated;
    }

    /// <summary>
    /// Called when any button has been pressed.
    /// </summary>
    private void Pressed()
    {
        CurrentPressed++;

        if(CurrentPressed == ButtonsRequired)
        {
            InternalActivate();
        }
    }

    /// <summary>
    /// Called when any button has been released.
    /// </summary>
    private void Released()
    {
        CurrentPressed--;

        if(CurrentPressed == ButtonsRequired - 1)
        {
            InternalDeactivate();
        }
    }

    /// <summary>
    /// Manages some activation logic, mainly keeping track of whether the object is activated or not.
    /// </summary>
    private void InternalActivate()
    {
        if(!NowActivated)
        {
            NowActivated = true;

            Activated();
        }
    }

    /// <summary>
    /// Manages some deactivated logic, mainly keeping track of whether the object is activated or not.
    /// </summary>
    private void InternalDeactivate()
    {
        if (NowActivated)
        {
            NowActivated = false;

            Deactivated();
        }
    }

    #endregion

}
