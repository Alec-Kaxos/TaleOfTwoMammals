using UnityEngine;
using UnityEngine.InputSystem;

// PlayerController class handles the players' inputs
public class PlayerController : MonoBehaviour
{
    protected PlayerInputs playerInputs;
    protected Rigidbody2D RB;
    [SerializeField]
    protected LayerMask GroundLayerMask;
    [SerializeField]
    protected Animator animator;

#region Movement Variables

    [Header("Movement Variables")]

    protected float horizontalInput = 0;
    [SerializeField]
    protected float movementVelocity = 5;
    [SerializeField]
    protected float jumpVelocity = 5;
    [SerializeField]
    private float smoothInputSpeed = .2f;

#endregion

    private Vector2 currentVector;
    private Vector2 smoothInputVelocity;

    private void Awake()
    {
        playerInputs = new PlayerInputs();
        RB = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        playerInputs.Enable();
        Subscribe();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
        Unsubscribe();
    }

    protected virtual void FixedUpdate()
    {
        Vector2 input = new Vector2(horizontalInput * movementVelocity, RB.velocity.y);
        currentVector = Vector2.SmoothDamp(currentVector, input, ref smoothInputVelocity, smoothInputSpeed);
        if (RB.bodyType != RigidbodyType2D.Static)
        {
            RB.velocity = new Vector2(currentVector.x, currentVector.y);
        }     
    }

#region Subscribe and Unsubscribe

    // This method subscribes corresponding methods to the
    // InputAction event to listen to any key press.
    protected virtual void Subscribe()
    {
    }
    

    protected virtual void Unsubscribe()
    {
    }
    

#endregion

#region Callback Events

    // This method is called when player press the movement key
    protected virtual void OnMoveStarted(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>().x > 0)
        {
            horizontalInput = 1;
        }
        else if (context.ReadValue<Vector2>().x < 0)
        {
            horizontalInput = -1;
        }
        else
        {
            horizontalInput = 0;
        }

        // Face the character to left or right 
        // It seems a little dumb how I do it
        if (horizontalInput == 1)
        {
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180f, 0));
        }
        else if (horizontalInput == -1)
        {
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }

    // This method is called when player release the movement key
    protected virtual void OnMoveCanceled(InputAction.CallbackContext context)
    {
        horizontalInput = 0;
    }

    protected virtual void OnJumpStarted(InputAction.CallbackContext context)
    {
        if(IsGrounded() && context.started)
            RB.velocity = new Vector2(RB.velocity.x, jumpVelocity);
    }

    protected virtual void OnJumpCanceled(InputAction.CallbackContext context)
    {
        if(context.started)
            RB.velocity = new Vector2(RB.velocity.x, 0);
    }

#endregion

#region Utility Methods

    // Check if player is on the ground or not
    protected bool IsGrounded()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        RaycastHit2D raycast = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.down, 0.1f, GroundLayerMask);
        return raycast.collider != null;
    }

    public void StopCharacter()
    {
        horizontalInput = 0;
        currentVector = Vector2.zero;
        RB.velocity = Vector2.zero;
    }

#endregion

    public void OnDeath()
    {
        StopCharacter();
        Unsubscribe();
        animator.SetTrigger("Die");
    }

    public void OnRespawn()
    {
        // Right now it only triggers the animator to go back 
        // to Idle state, but we might want to do more
        animator.SetTrigger("Respawn");
        Subscribe();
    }
}
