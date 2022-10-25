using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

// PlayerController class handles the players' inputs
public class PlayerController : MonoBehaviour
{
    protected PlayerInputs playerInputs;
    protected Rigidbody2D RB;
    [SerializeField]
    protected LayerMask GroundLayerMask;

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

    private void FixedUpdate()
    {
        Vector2 input = new Vector2(horizontalInput * movementVelocity, RB.velocity.y);
        currentVector = Vector2.SmoothDamp(currentVector, input, ref smoothInputVelocity, smoothInputSpeed);
        RB.velocity = new Vector2(currentVector.x, currentVector.y);
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
        horizontalInput = context.ReadValue<Vector2>().x;
    }

    // This method is called when player release the movement key
    protected virtual void OnMoveCanceled(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<Vector2>().x;
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
    private bool IsGrounded()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        RaycastHit2D raycast = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.down, 0.1f, GroundLayerMask);
        return raycast.collider != null;
    }

#endregion
}
