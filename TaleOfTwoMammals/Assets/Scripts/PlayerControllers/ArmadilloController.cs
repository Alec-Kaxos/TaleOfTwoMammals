using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmadilloController: PlayerController
{
    protected override void Subscribe()
    {
        playerInputs.Armadillo.Move.started += OnMoveStarted;
        playerInputs.Armadillo.Move.canceled += OnMoveCanceled;

        playerInputs.Armadillo.Jump.started += OnJumpStarted;
        playerInputs.Armadillo.Jump.canceled += OnJumpCanceled;
    }

    protected override void Unsubscribe()
    {
        playerInputs.Armadillo.Move.started -= OnMoveStarted;
        playerInputs.Armadillo.Move.canceled -= OnMoveCanceled;

        playerInputs.Armadillo.Jump.started -= OnJumpStarted;
        playerInputs.Armadillo.Jump.canceled -= OnJumpCanceled;
    }
}
