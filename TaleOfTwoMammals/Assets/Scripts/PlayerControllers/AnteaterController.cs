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
    [SerializeField]
    private float tongueLength = 20f;
    [SerializeField]
    private LayerMask tongueLayers;

    #endregion

    #region Tongue Bridge

    [SerializeField]
    private float tongueWidth = .1f;
    private bool tongueOut = false;
    private GameObject tongueBridge = null;
    [SerializeField]
    private Texture2D tongueTexture;

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
        // Check if Anteater is aiming or has tongue out, Anteater will not be able to move if it is aiming
        if (isAiming == false && !tongueOut)
        {
            base.OnMoveStarted(context);
        }
    }

    protected override void OnJumpStarted(InputAction.CallbackContext context)
    {
        // Check if Anteater is aimingor has tongue out, Anteater will not be able to jump if it is aiming
        if (isAiming == false && !tongueOut)
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
        //First, check if the tongue bridge is already out, if so, recall it
        if (tongueOut)
        {
            despawnTongueBridge();
        }
        else //The tongue bridge is not currently deployed, so start aiming.
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

                //CHANGE DISTANCE
                RaycastHit2D hit = Physics2D.Raycast(transform.position, aimingSprites.transform.up, tongueLength, tongueLayers);
                if (hit.collider != null)
                {
                    Debug.Log(hit.collider.gameObject);
                    Debug.DrawRay(transform.position, new Vector3(hit.point.x, hit.point.y, 0f) - transform.position, Color.red, 5.0f);
                    //Likely insert object type check here
                    spawnTongueBridge(hit.point);

                }

                aimingSprites.SetActive(false);
            }
        }
    }

    private void spawnTongueBridge(Vector2 endpoint)
    {
        tongueOut = true;

        //Deal with movement lock
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        float tongueLen = (endpoint - new Vector2(transform.position.x, transform.position.y)).magnitude;
        //Rect tongueRect = new Rect(transform.position.x, transform.position.y, tongueWidth, tongueLen);

        //Set tongue dimensions
        tongueBridge = new GameObject("Tongue Bridge");
        tongueBridge.transform.localScale = new Vector3(tongueLen, tongueWidth, 1f);
        tongueBridge.transform.rotation = aimingSprites.transform.rotation;
        tongueBridge.transform.Rotate(0, 0, 90);
        tongueBridge.transform.SetParent(gameObject.transform);
        tongueBridge.transform.localPosition = new Vector3();
        BoxCollider2D tBoxC = tongueBridge.AddComponent<BoxCollider2D>();
        tBoxC.offset = new Vector2(.5f, .5f);
        tongueBridge.layer = LayerMask.NameToLayer("Ground");

        //Set Sprite
        Texture2D test = new Texture2D(100, 100);
        Sprite sprite = Sprite.Create(test, new Rect(0, 0, 100, 100), new Vector2());
        SpriteRenderer renderer = tongueBridge.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;

    }

    private void despawnTongueBridge()
    {
        tongueOut = false;

        //Deal with movement lock
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        //Destroy tongue
        Destroy(tongueBridge);
        tongueBridge = null;
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

        Debug.DrawRay(transform.position, tongueLength* aimingSprites.transform.up );

        if (isAiming == true)
        {
            Vector3 currentRotation = aimingSprites.transform.rotation.eulerAngles;
            aimingSprites.transform.Rotate(new Vector3(0, 0, aimingSpriteRotatingSpeed * aimingRotationInput * rotationDirection));
        }

#endregion
    }
}
