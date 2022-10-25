using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnteaterController : PlayerController
{
    private bool isAiming = false;

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
    }

#endregion
}
