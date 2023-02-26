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

    protected Vector2 slopeNormal = Vector2.zero;

    [SerializeField]
    protected AudioSource AnteaterJumpSound;

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
        CheckGround();
        if (IsGrounded() && !IsOnMovableSlope())
        {
            return;
        }
        Vector2 input = new Vector2(horizontalInput * movementVelocity, RB.velocity.y);
        currentVector = Vector2.SmoothDamp(currentVector, input, ref smoothInputVelocity, smoothInputSpeed);
        if (RB.bodyType != RigidbodyType2D.Static)
        {
            RB.velocity = new Vector2(currentVector.x, currentVector.y);
        }
    }

    protected virtual void UpdateAnimation()
    {
        if (RB.velocity.x > (movementVelocity - 1.5) || RB.velocity.x < (-movementVelocity + 1.5))
        {
            animator.SetBool("Moving", true);

            if (RB.velocity.x > (movementVelocity - 0.6) || RB.velocity.x < (-movementVelocity + 0.6))
            {
                animator.SetBool("Pushing", false);
            }
            else
            {
                animator.SetBool("Pushing", true);
            }
        }
        else
        {
            animator.SetBool("Moving", false);
            animator.SetBool("Pushing", false);
        }

        if (!IsGrounded())
        {
            if(RB.velocity.y > 0.1)
            {
                animator.SetBool("Jumping", true);
            }
            else if(RB.velocity.y < -1)
            {
                animator.SetBool("Falling", true);
            }
        }

        else
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", false);
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
        CheckGround();
        if (IsGrounded() && IsOnMovableSlope() && context.started)
        {
            RB.velocity = new Vector2(RB.velocity.x, jumpVelocity);
            AnteaterJumpSound.Play();

        }
    }

    protected virtual void OnJumpCanceled(InputAction.CallbackContext context)
    {
        if(context.started)
            RB.velocity = new Vector2(RB.velocity.x, 0);
    }

    public virtual void OnDeath()
    {
        StopCharacter();
        Unsubscribe();
        animator.SetTrigger("Die");
    }

    public virtual void OnRespawn()
    {
        ResetCharacter();
        Subscribe();
    }

#endregion

#region Utility Methods

    // Call this method before calling IsGrounded() or IsOnMovableSlope()
    // If you try to call IsOnMovableSlope() to check if is on a slope with
    // allowed angle, call IsGrounded() && IsOnMovableSlope() in order instead
    protected void CheckGround()
    {
        RaycastHit2D raycast = Physics2D.BoxCast(detectionCollider.bounds.center, detectionCollider.bounds.size, 0f, Vector2.down, 0.1f, GroundLayerMask);
        if (raycast.collider != null)
        {
            slopeNormal = raycast.normal;
        }
        else
        {
            slopeNormal = Vector2.zero;
        }
    }

    // Check if player is on the ground or not
    protected bool IsGrounded()
    {
        return slopeNormal != Vector2.zero;
    }

    // Check if player is on a slope with allowed angle
    protected bool IsOnMovableSlope()
    {
        return Vector2.Angle(slopeNormal, Vector2.up) <= maxClimbAngle;
    }

    public void StopCharacter()
    {
        horizontalInput = 0;
        currentVector = Vector2.zero;
        RB.velocity = Vector2.zero;
    }

    // Reset the character back to normal state
    public virtual void ResetCharacter()
    {

    }

#endregion

    public void SetLevelManager(LevelManager l)
    {
        LM = l;
    }
    
    public LevelManager GetLevelManager()
    {
        return LM;
    }
}
