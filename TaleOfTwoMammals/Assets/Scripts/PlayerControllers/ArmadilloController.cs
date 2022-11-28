using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmadilloController : PlayerController
{
    // False means the Armadillo is in the normal form
    // True means the Armadillo has transformed into the ball
    private bool transformed = false;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    [Tooltip("How much more speed you want the Armadillow to have after transforming into a ball?")]
    [Range(1, 2)]
    private float speedMultiplier = 1.25f;

    [SerializeField]
    private LayerMask tunnelLayerMask;

#region Normal Form Components

    [Header("Normal Form Components")]
    [Tooltip("This stands for the collider when the Armadillo is in its normal form. This should be set to be ACTIVE by default.")]
    [SerializeField]
    private BoxCollider2D normalCollider;
    [Tooltip("This stands for the sprite when the Armadillo is in its normal form.")]
    [SerializeField]
    private Sprite normalSprite;

#endregion

#region Ball Form Components

    [Header("Ball Form Components")]
    [Tooltip("This stands for the collider when the Armadillo transforms into the ball. This should be set to be INACTIVE by default.")]
    [SerializeField]
    private BoxCollider2D ballCollider;
    [Tooltip("This stands for the sprite when the Armadillo transforms into the ball.")]
    //[SerializeField]
    //private Sprite ballSprite;
    #endregion

#region GroundPound Components
    [SerializeField]
    protected LayerMask PoundLayers;
    private Vector2 PoundSpeed = new Vector2(0f, -20f);
    private bool Pounding = false;
#endregion

    protected override void Awake()
    {
        base.Awake();
        detectionCollider = normalCollider;
    }

#region Subscribe and Unsubscribe

    protected override void Subscribe()
    {
        playerInputs.Armadillo.Move.started += OnMoveStarted;
        playerInputs.Armadillo.Move.canceled += OnMoveCanceled;

        playerInputs.Armadillo.Jump.started += OnJumpStarted;
        playerInputs.Armadillo.Jump.canceled += OnJumpCanceled;

        playerInputs.Armadillo.Transform.started += OnTransform;
    }

    protected override void Unsubscribe()
    {
        playerInputs.Armadillo.Move.started -= OnMoveStarted;
        playerInputs.Armadillo.Move.canceled -= OnMoveCanceled;

        playerInputs.Armadillo.Jump.started -= OnJumpStarted;
        playerInputs.Armadillo.Jump.canceled -= OnJumpCanceled;

        playerInputs.Armadillo.Transform.started -= OnTransform;
    }

#endregion

#region Armadillo Special Methods

    protected override void OnJumpStarted(InputAction.CallbackContext context)
    {
        if (transformed == false)
        {
            base.OnJumpStarted(context);
        }
        else
        {
            OnTransform(context);
        }
    }
 
    private void GroundPound()
    {
        RB.velocity = PoundSpeed;
        Pounding = true;
    }

    private void FinishedPounding()
    {
        Pounding = false;
    }

    private void toggleTransform()
    {
        transformed = !transformed;
    }

    private void OnTransform(InputAction.CallbackContext context)
    {
        toggleTransform();
        if (transformed)
        {
            ballCollider.enabled = true;
            normalCollider.enabled = false;
            detectionCollider = ballCollider;

            animator.SetBool("BallTrigger", true);
            //spriteRenderer.sprite = ballSprite;
            movementVelocity *= speedMultiplier;

            //Gives the Armadillo the ability to use a groundpound move.
            if (!IsGrounded())
            {
                RaycastHit2D hit = Physics2D.Raycast(RB.position, -RB.transform.up, 20f, PoundLayers);
                float DistanceFromGround = RB.position.y - hit.point.y;
                Debug.Log(DistanceFromGround);
                if (DistanceFromGround > 3.5f)
                {
                    GroundPound();
                }            
            }

        }
        else
        {
            if (IsInTunnel())
            {
                toggleTransform();
                return;
            }
            animator.SetBool("BallTrigger", false);
            ballCollider.enabled = false;
            normalCollider.enabled = true;
            detectionCollider = normalCollider;
            //spriteRenderer.sprite = normalSprite;
            movementVelocity /= speedMultiplier;
        }
    }

    /// <summary>
    /// Takes the Armadillo out of ball form, if it is in it.
    /// </summary>
    private void ResetBall()
    {
        if (transformed)
        {
            animator.SetBool("BallTrigger", false);
            ballCollider.enabled = false;
            normalCollider.enabled = true;
            //spriteRenderer.sprite = normalSprite;
            movementVelocity /= speedMultiplier;
            detectionCollider = normalCollider;

            transformed = false;
        }
    }

#endregion

#region Utilities

    private bool IsInTunnel()
    {
        RaycastHit2D raycast = Physics2D.BoxCast(ballCollider.bounds.center, ballCollider.bounds.size, 0f, Vector2.up, 0.1f, tunnelLayerMask);
        return raycast.collider != null;
    }

    public override void ResetCharacter()
    {
        base.ResetCharacter();
        ResetBall();
    }

#endregion

#region Public Methods

    public bool IsInBallForm()
    {
        return transformed;
    }

    public bool IsPounding()
    {
        if (Pounding)
        {
            return true;
        }
        return false;
    }

    public override void OnDeath()
    {
        base.OnDeath();
        ResetBall();
    }

#endregion


    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (RB.velocity.y > -0.01)
        {
            FinishedPounding();
        }
    }
}
