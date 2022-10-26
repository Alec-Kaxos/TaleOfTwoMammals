using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnteaterController : PlayerController
{
    private bool isAiming = false;
    private int rotationDirection = 1;

#region Aiming

    [Header("Aiming")]
    [SerializeField]
    private GameObject aimingSprites;
    [SerializeField]
    private float aimingSpriteRotatingSpeed = 1f;
    private float aimingRotationInput = 0;

#endregion

#region Subscribe and Unsubscribe

    protected override void Subscribe()
    {
        playerInputs.Anteater.Move.started += OnMoveStarted;
        playerInputs.Anteater.Move.canceled += OnMoveCanceled;

        playerInputs.Anteater.Jump.started += OnJumpStarted;
        playerInputs.Anteater.Jump.canceled += OnJumpCanceled;

        playerInputs.Anteater.ShootTongue.canceled += OnShootTongue;
    }

    protected override void Unsubscribe()
    {
        playerInputs.Anteater.Move.started -= OnMoveStarted;
        playerInputs.Anteater.Move.canceled -= OnMoveCanceled;

        playerInputs.Anteater.Jump.started -= OnJumpStarted;
        playerInputs.Anteater.Jump.canceled -= OnJumpCanceled;

        playerInputs.Anteater.ShootTongue.canceled -= OnShootTongue;
    }

#endregion

#region Anteater Special Abilities

    protected override void OnMoveStarted(InputAction.CallbackContext context)
    {
        // Check if Anteater is aiming, Anteater will not be able to move if it is aiming
        if (isAiming == false)
        {
            base.OnMoveStarted(context);
        }
    }

    protected override void OnJumpStarted(InputAction.CallbackContext context)
    {
        // Check if Anteater is aiming, Anteater will not be able to jump if it is aiming
        if (isAiming == false)
        {
            base.OnJumpStarted(context);
        }
    }

    private void toggleIsAiming()
    {
        isAiming = !isAiming;
    }

    private void OnShootTongue(InputAction.CallbackContext context)
    {
        toggleIsAiming();
        if (isAiming == true)
        {
            playerInputs.Anteater.Aim.started += OnAim;
            // Unsubscribe movement from player movement inputs
            playerInputs.Anteater.Move.started -= OnMoveStarted;
            playerInputs.Anteater.Move.canceled -= OnMoveCanceled;
            // Subscribe pointer rotation to player movement inputs
            playerInputs.Anteater.Move.started += OnMovePointerStarted;
            playerInputs.Anteater.Move.canceled += OnMovePointerCanceled;

            aimingSprites.SetActive(true);
        }
        else
        {
            playerInputs.Anteater.Aim.started -= OnAim;
            // Subscribe movement to player movement inputs
            playerInputs.Anteater.Move.started += OnMoveStarted;
            playerInputs.Anteater.Move.canceled += OnMoveCanceled;
            // Unsubscribe pointer rotation from player movement inputs
            playerInputs.Anteater.Move.started -= OnMovePointerStarted;
            playerInputs.Anteater.Move.canceled -= OnMovePointerCanceled;

            aimingSprites.SetActive(false);
        }
    }

    private void OnMovePointerStarted(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>().x > 0)
        {
            aimingRotationInput = 1;
        }
        else if (context.ReadValue<Vector2>().x < 0)
        {
            aimingRotationInput = -1;
        }
        else
        {
            aimingRotationInput = 0;
        }

        // Just to make pointer always rotate clockwise/counter clockwise when player is
        // press right/left no matter which way the charater is facing
        if (gameObject.transform.rotation.y < 0)
        {
            rotationDirection = 1;
        }
        else
        {
            rotationDirection = -1;
        }
    }

    private void OnMovePointerCanceled(InputAction.CallbackContext context)
    {
        aimingRotationInput = 0;
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        Vector3 rotateAngle = context.ReadValue<Vector3>();
        aimingSprites.transform.Rotate(rotateAngle);
    }

#endregion

    private void LateUpdate()
    {
#region Rotating the Aiming Sprites

        if (isAiming == true)
        {
            Vector3 currentRotation = aimingSprites.transform.rotation.eulerAngles;
            aimingSprites.transform.Rotate(new Vector3(0, 0, aimingSpriteRotatingSpeed * aimingRotationInput * rotationDirection));
        }

#endregion
    }
}
