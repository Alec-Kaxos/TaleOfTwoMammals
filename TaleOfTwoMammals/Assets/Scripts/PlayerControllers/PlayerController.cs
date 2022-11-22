using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;

// PlayerController class handles the players' inputs
public class PlayerController : MonoBehaviour
{
    protected PlayerInputs playerInputs;
    protected Rigidbody2D RB;
    [SerializeField]
    protected LayerMask GroundLayerMask;
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected Collider2D detectionCollider;

    [SerializeField]
    protected LevelManager LM;

#region Movement Variables

    [Header("Movement Variables")]

    protected float horizontalInput = 0;
    [SerializeField]
    protected float movementVelocity = 5;
    [SerializeField]
    protected float jumpVelocity = 5;
    [SerializeField]
    private float smoothInputSpeed = .2f;
    [SerializeField]
    protected float maxClimbAngle = 30f;

#endregion

    private Vector2 currentVector;
    private Vector2 smoothInputVelocity;

    protected virtual void Awake()
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
        if (IsOnMovableSlope())
        {
            Vector2 input = new Vector2(horizontalInput * movementVelocity, RB.velocity.y);
            currentVector = Vector2.SmoothDamp(currentVector, input, ref smoothInputVelocity, smoothInputSpeed);
            if (RB.bodyType != RigidbodyType2D.Static)
            {
                RB.velocity = new Vector2(currentVector.x, currentVector.y);
            }
        }
    }

    protected virtual void UpdateAnimation()
    {
        if (RB.velocity.x > 0.01 || RB.velocity.x < -0.01)
        {
            animator.SetBool("Moving", true);
        }
        else
        {
            animator.SetBool("Moving", false);
        }
    }

    protected virtual void LateUpdate()
    {
        UpdateAnimation();
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
        if(IsOnMovableSlope() && context.started)
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
        RaycastHit2D raycast = Physics2D.BoxCast(detectionCollider.bounds.center, detectionCollider.bounds.size, 0f, Vector2.down, 0.1f, GroundLayerMask);
        return raycast.collider != null;
    }

    protected bool IsOnMovableSlope()
    {
        RaycastHit2D raycast = Physics2D.BoxCast(detectionCollider.bounds.center, detectionCollider.bounds.size, 0f, Vector2.down, 0.1f, GroundLayerMask);
        // Debug.Log(string.Format("{0}: raycast is not null: {1}, smaller than max angle is: {2}, collider is not active: {3}", gameObject.name, raycast.collider != null, Vector2.Angle(raycast.normal, Vector2.up) <= maxClimbAngle, detectionCollider.isActiveAndEnabled));
        return (raycast.collider != null && Vector2.Angle(raycast.normal, Vector2.up) <= maxClimbAngle);
    }

    public void StopCharacter()
    {
        horizontalInput = 0;
        currentVector = Vector2.zero;
        RB.velocity = Vector2.zero;
    }

#endregion

    public virtual void OnDeath()
    {
        StopCharacter();
        Unsubscribe();
        animator.SetTrigger("Die");
    }

    public void OnRespawn()
    {
        Subscribe();
    }

    public void SetLevelManager(LevelManager l)
    {
        LM = l;
    }
    
    public LevelManager GetLevelManager()
    {
        return LM;
    }
}
