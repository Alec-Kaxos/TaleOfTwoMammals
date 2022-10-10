using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnteaterController : PlayerController
{
    protected override void Subscribe()
    {
        playerInputs.Anteater.Move.started += OnMoveStarted;
        playerInputs.Anteater.Move.canceled += OnMoveCanceled;

        playerInputs.Anteater.Jump.started += OnJumpStarted;
        playerInputs.Anteater.Jump.canceled += OnJumpCanceled;
    }

    protected override void Unsubscribe()
    {
        playerInputs.Anteater.Move.started -= OnMoveStarted;
        playerInputs.Anteater.Move.canceled -= OnMoveCanceled;

        playerInputs.Anteater.Jump.started -= OnJumpStarted;
        playerInputs.Anteater.Jump.canceled -= OnJumpCanceled;
    }
}
