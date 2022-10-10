using UnityEngine;
using UnityEngine.InputSystem;

// PlayerController class handles the players' inputs
// Note: 
// I intended to make this class an parent class with abstract method,
// how it is written now is just to show how it will work
public class PlayerController : MonoBehaviour
{
    protected PlayerInputs playerInputs;
    protected Rigidbody2D RB;
    [SerializeField]
    protected LayerMask GroundLayerMask;

    #region Movement Variables

    [Header("Movement Variables")]

    protected float HorizontalInput = 0;
    [SerializeField]
    protected float MovementVelocity = 5;
    [SerializeField]
    protected float JumpVelocity = 5;

    #endregion

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
        RB.velocity = new Vector2(HorizontalInput* MovementVelocity, RB.velocity.y);
    }

    #region Subscribe and Unsubscribe

    // This method subscribes corresponding methods to the
    // InputAction event to listen to any key press.
    // Note:
    // I used playerInputs. "Anteater"  below, because
    // I made another ActionMap in "PlayerInputs" called
    // "Armadillo", and if I am doing this correctly,
    // the Subscribe() and Unsubscribe() should be *virtual*
    protected virtual void Subscribe()
    {
    }
    

    protected virtual void Unsubscribe()
    {
    }
    

    #endregion

    #region Callback Events

    // This method is called when player press the movement key
    protected void OnMoveStarted(InputAction.CallbackContext context)
    {
        HorizontalInput = context.ReadValue<Vector2>().x;
    }

    // This method is called when player release the movement key
    protected void OnMoveCanceled(InputAction.CallbackContext context)
    {
        HorizontalInput = context.ReadValue<Vector2>().x;
    }

    protected void OnJumpStarted(InputAction.CallbackContext context)
    {
        if (IsGrounded() && context.started)
            RB.velocity = new Vector2(RB.velocity.x, JumpVelocity);
    }

    protected void OnJumpCanceled(InputAction.CallbackContext context)
    {
        RB.velocity = new Vector2(RB.velocity.x, 0);
    }

    #endregion

    #region Utility Methods

    // Check if player is on the ground or not
    private bool IsGrounded()
    {
        Collider2D collider = GetComponent<BoxCollider2D>();
        RaycastHit2D raycast = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.down, 0.1f, GroundLayerMask);
        return raycast.collider != null;
    }

    #endregion
}
