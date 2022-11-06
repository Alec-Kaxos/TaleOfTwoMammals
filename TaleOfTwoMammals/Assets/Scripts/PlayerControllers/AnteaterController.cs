using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnteaterController : PlayerController
{
    private bool isAiming = false;
    private int rotationDirection = 1;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite normalSprite;
    [SerializeField]
    private Sprite crawlSprite;
    [SerializeField]
    private Transform tongueStartPointRef;

#region Aiming

    [Header("Aiming")]
    [SerializeField]
    private GameObject aimingSprites;
    [SerializeField]
    private GameObject crosshairSprite;
    private RaycastHit2D crosshairLocation;
    [SerializeField]
    private float aimingSpriteRotatingSpeed = 1f;
    private float aimingRotationInput = 0;
    [SerializeField]
    private float tongueLength = 20f;
    [SerializeField]
    private LayerMask tongueLayers;

#endregion

#region Tongue Bridge Variables

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

            spriteRenderer.sprite = normalSprite;
        }
        else if (IsGrounded())//The tongue bridge is not currently deployed, so start aiming.
        {
            StopCharacter();

            toggleIsAiming();
            if (isAiming == true)
            {
                // Unsubscribe movement from player movement inputs
                playerInputs.Anteater.Move.started -= OnMoveStarted;
                playerInputs.Anteater.Move.canceled -= OnMoveCanceled;
                // Unsubscribe Jump
                playerInputs.Anteater.Jump.started -= OnJumpStarted;
                playerInputs.Anteater.Jump.canceled -= OnJumpCanceled;
                // Subscribe pointer rotation to player movement inputs
                playerInputs.Anteater.Move.started += OnMovePointerStarted;
                playerInputs.Anteater.Move.canceled += OnMovePointerCanceled;

                aimingSprites.SetActive(true);

                aimingSprites.transform.parent = tongueStartPointRef.transform;
                aimingSprites.transform.localPosition = new Vector3(0f,0f,0f);

                // Sets up the crosshair to appear
                crosshairSprite.transform.parent = tongueStartPointRef.transform;
                crosshairSprite.transform.localPosition = new Vector3(0f, 0f, 0f);


                spriteRenderer.sprite = crawlSprite;
            }
            else
            {
                // Subscribe movement to player movement inputs
                playerInputs.Anteater.Move.started += OnMoveStarted;
                playerInputs.Anteater.Move.canceled += OnMoveCanceled;
                // Subscribe Jump
                playerInputs.Anteater.Jump.started += OnJumpStarted;
                playerInputs.Anteater.Jump.canceled += OnJumpCanceled;
                // Unsubscribe pointer rotation from player movement inputs
                playerInputs.Anteater.Move.started -= OnMovePointerStarted;
                playerInputs.Anteater.Move.canceled -= OnMovePointerCanceled;


                spriteRenderer.sprite = normalSprite;
                //CHANGE DISTANCE
                RaycastHit2D hit = Physics2D.Raycast(tongueStartPointRef.position, aimingSprites.transform.up, tongueLength, tongueLayers);
                if (hit.collider != null)
                {
                    Debug.Log(hit.collider.gameObject);
                    Debug.DrawRay(tongueStartPointRef.position, new Vector3(hit.point.x, hit.point.y, 0f) - tongueStartPointRef.position, Color.red, 5.0f);
                    //Likely insert object type check here
                    spawnTongueBridge(hit.point);

                    spriteRenderer.sprite = crawlSprite;
                }

                aimingSprites.SetActive(false);
                crosshairSprite.SetActive(false);

                // Makes sure aiming pointer is not rotating on start of next aiming
                aimingRotationInput = 0;

                aimingSprites.transform.parent = tongueStartPointRef.transform;
                // Reset the aiming sprite to point upwards again
                aimingSprites.transform.rotation = new Quaternion(0, 0, 0, 0);
            }
        }
    }

    private void spawnTongueBridge(Vector2 endpoint)
    {
        tongueOut = true;

        //Deal with movement lock
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        GetComponent<BoxCollider2D>().enabled = false;

        float tongueLen = (endpoint - new Vector2(tongueStartPointRef.position.x, tongueStartPointRef.position.y)).magnitude;
        //Rect tongueRect = new Rect(transform.position.x, transform.position.y, tongueWidth, tongueLen);

        //Set tongue dimensions
        tongueBridge = new GameObject("Tongue Bridge");
        tongueBridge.transform.localScale = new Vector3(tongueLen, tongueWidth, 1f);
        tongueBridge.transform.rotation = aimingSprites.transform.rotation;
        tongueBridge.transform.Rotate(0, 0, 90);
        tongueBridge.transform.SetParent(tongueStartPointRef);
        tongueBridge.transform.localPosition = new Vector3(0f, tongueWidth/2, 0f);
        BoxCollider2D tBoxC = tongueBridge.AddComponent<BoxCollider2D>();
        tBoxC.offset = new Vector2(.5f, .5f);
        tongueBridge.layer = LayerMask.NameToLayer("Ground");

        //Set Sprite
        Texture2D test = new Texture2D(100, 100);
        Sprite sprite = Sprite.Create(test, new Rect(0, 0, 100, 100), new Vector2());
        SpriteRenderer renderer = tongueBridge.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = Color.red;

    }

    private void despawnTongueBridge()
    {
        tongueOut = false;

        //Deal with movement lock
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<BoxCollider2D>().enabled = true;


        //Destroy tongue
        Destroy(tongueBridge);
        tongueBridge = null;
    }

    private void OnMovePointerStarted(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>().x > 0)
        {
            aimingRotationInput = -1;
        }
        else if (context.ReadValue<Vector2>().x < 0)
        {
            aimingRotationInput = 1;
        }
        else
        {
            aimingRotationInput = 0;
        }
    }

    private void OnMovePointerCanceled(InputAction.CallbackContext context)
    {
        aimingRotationInput = 0;
    }

#endregion

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

#region Rotating the Aiming Sprites along with the Anteater

        Debug.DrawRay(transform.position, tongueLength* aimingSprites.transform.up );

        if (isAiming == true)
        {
            Vector3 currentRotation = aimingSprites.transform.rotation.eulerAngles;
            float zValue = currentRotation.z + aimingSpriteRotatingSpeed * aimingRotationInput;
            aimingSprites.transform.rotation = Quaternion.Euler(new Vector3(0, 0, zValue));

            // Sets the Location of the crosshair and makes it visible to the player
            crosshairLocation = Physics2D.Raycast(tongueStartPointRef.position, aimingSprites.transform.up, tongueLength, tongueLayers);
            if (crosshairLocation.collider != null)
            {
                crosshairSprite.SetActive(true);
                crosshairSprite.transform.position = crosshairLocation.point;
            }
            else
            {
                crosshairSprite.SetActive(false);
            }

            // Rotating the Anteater according to the rotation of the pointer
            // Excluding 0 and 360 here since rotation starts at 0, and we want 
            // the Anteater to keep its direction when starting to aim
            if (zValue > 180 && zValue < 360)
            {
                // rotate to left
                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180f, 0));
            }
            else if (zValue > 0 &&zValue < 180)
            {
                // rotate to right
                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }

#endregion
    }

#region Utilities

    private void DetermineRotationDirection()
    {
        // Just to make pointer always rotate clockwise/counter clockwise when player is
        // press right/left no matter which way the charater is facing
        if (gameObject.transform.rotation.y < 0)
        {
            rotationDirection = -1;
        }
        else
        {
            rotationDirection = 1;
        }
    }

#endregion
}
