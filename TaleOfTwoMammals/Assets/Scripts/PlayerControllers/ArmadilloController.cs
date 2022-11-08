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

    // Armadillo will not be able to jump when it is currenly in the ball form
    protected override void OnJumpStarted(InputAction.CallbackContext context)
    {
        if (transformed == false)
        {
            base.OnJumpStarted(context);
        }
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

            animator.SetBool("BallTrigger", true);
            //spriteRenderer.sprite = ballSprite;
            movementVelocity *= speedMultiplier;
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
            //spriteRenderer.sprite = normalSprite;
            movementVelocity /= speedMultiplier;
        }
    }

#endregion

#region Utilities

    private bool IsInTunnel()
    {
        RaycastHit2D raycast = Physics2D.BoxCast(ballCollider.bounds.center, ballCollider.bounds.size, 0f, Vector2.up, 0.1f, tunnelLayerMask);
        return raycast.collider != null;
    }

    #endregion

#region Public Methods

    public bool IsInBallForm()
    {
        return transformed;
    }

    #endregion


    private void LateUpdate()
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
}
