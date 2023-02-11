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

    [Header("Crouching")]
    [SerializeField]
    private Sprite normalSprite;
    [SerializeField]
    private Sprite crawlSprite;
    [SerializeField]
    private BoxCollider2D normalCollider;
    private BoxCollider2D normalColliderCopy;
    [SerializeField]
    private BoxCollider2D crouchCollider;

    [SerializeField]
    private Transform tongueStartPointRef;

    private bool isInCoroutine = false;

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

    //Used for the tongue growth
    [SerializeField]
    private float baseTongueShootTime = 0.5f;
    public Vector2 tongueEndPoint;
    private float growthTimer = 0f;

#endregion

    protected override void Awake()
    {
        base.Awake();
        detectionCollider = normalCollider;
    }

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
        CheckGround();
        if (isInCoroutine)
		{
            return;
		}
        //First, check if the tongue bridge is already out, if so, recall it
        if (tongueOut)
        {
            /* original
            // despawnTongueBridge();
            // Uncrouch();
            */
            RetractTongue();
        }
        else if (IsGrounded() && IsOnMovableSlope())//The tongue bridge is not currently deployed, so start aiming.
        {

            StopCharacter();

            toggleIsAiming();
            if (isAiming == true)
            {
                HijackInputFromMovement();

                aimingSprites.SetActive(true);

                aimingSprites.transform.parent = tongueStartPointRef.transform;
                aimingSprites.transform.localPosition = new Vector3(0f,0f,0f);

                // Sets up the crosshair to appear
                crosshairSprite.transform.parent = tongueStartPointRef.transform;
                crosshairSprite.transform.localPosition = new Vector3(0f, 0f, 0f);

                Crouch();
            }
            else
            {
                ReturnInputBackToMovement();

                //CHANGE DISTANCE
                RaycastHit2D hit = Physics2D.Raycast(tongueStartPointRef.position, aimingSprites.transform.up, tongueLength, tongueLayers);
                if (hit.collider != null)
                {
                    /*
                    Debug.Log(hit.collider.gameObject);
                    Debug.DrawRay(tongueStartPointRef.position, new Vector3(hit.point.x, hit.point.y, 0f) - tongueStartPointRef.position, Color.red, 5.0f);
                    */
                    //Likely insert object type check here

                    //Saves where tongue will hit instead of sending it to spawnTongueBridge()
                    tongueEndPoint = hit.point;
                    spawnTongueBridge();

                }
                else
                {
                    tongueEndPoint = aimingSprites.transform.up * tongueLength + tongueStartPointRef.position;
                    TongueBridgeUnavailableAction();
                    /* original
                    // Uncrouch();
                    */
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

    private void spawnTongueBridge()
    {
        //Deal with movement lock
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        //GetComponent<BoxCollider2D>().enabled = false;
        //float tongueLen = (endpoint - new Vector2(tongueStartPointRef.position.x, tongueStartPointRef.position.y)).magnitude;
        //Rect tongueRect = new Rect(transform.position.x, transform.position.y, tongueWidth, tongueLen);

        //Set tongue dimensions
        tongueBridge = new GameObject("Tongue Bridge");
        tongueBridge.transform.localScale = new Vector3(0f, tongueWidth, 1f);

        //Makes the tongue look like its shooting out
        //sorta
        StartCoroutine(Grow());


        tongueBridge.transform.rotation = aimingSprites.transform.rotation;
        tongueBridge.transform.Rotate(0, 0, 90);
        tongueBridge.transform.SetParent(tongueStartPointRef);
        tongueBridge.transform.localPosition = new Vector3(0f, tongueWidth/2, 0f);
        BoxCollider2D tBoxC = tongueBridge.AddComponent<BoxCollider2D>();
        Rigidbody2D rigidbody = tongueBridge.AddComponent<Rigidbody2D>();
        rigidbody.isKinematic = true;
        tBoxC.offset = new Vector2(.5f, .5f);
        tongueBridge.layer = LayerMask.NameToLayer("Ground");
        tongueBridge.tag = "Tongue";

        //Set Sprite
        Texture2D test = new Texture2D(100, 100);
        Sprite sprite = Sprite.Create(test, new Rect(0, 0, 100, 100), new Vector2());
        SpriteRenderer renderer = tongueBridge.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = Color.red;
    }

    private void despawnTongueBridge()
    {
        //Resets timer for tongue growth
        growthTimer = 0f;

        tongueOut = false;

        //Deal with movement lock
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        //GetComponent<BoxCollider2D>().enabled = true;


        //Destroy tongue
        Destroy(tongueBridge);
        tongueBridge = null;
    }

    private void RetractTongue()
    {
        StartCoroutine(Degrow());
    }

    private IEnumerator RetractTongueCoroutine()
    {
        while (tongueOut == false)
        {
            yield return null;
        }
        StartCoroutine(Degrow());
    }
    
    // bad name here
    private void TongueBridgeUnavailableAction()
    {
        spawnTongueBridge();
        StartCoroutine(RetractTongueCoroutine());
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

    private void Crouch()
    {
        animator.SetBool("Aiming", true);
        //spriteRenderer.sprite = crawlSprite;
        //crouchCollider.enabled = true;
        //normalCollider.enabled = false;
        if (normalColliderCopy == null)
        { //Its the first time setting this copy
            normalColliderCopy = gameObject.AddComponent<BoxCollider2D>();
            normalColliderCopy.enabled = false;
            normalColliderCopy.offset = normalCollider.offset;
            normalColliderCopy.size = normalCollider.size;
        }
        normalCollider.offset = crouchCollider.offset;
        normalCollider.size = crouchCollider.size;
    }

    private void Uncrouch()
    {
        animator.SetBool("Aiming", false);
        //spriteRenderer.sprite = normalSprite;
        //crouchCollider.enabled = false;
        //normalCollider.enabled = true;
        normalCollider.offset = normalColliderCopy.offset;
        normalCollider.size = normalColliderCopy.size;
    }

    private void HijackInputFromMovement()
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
    }

    private void ReturnInputBackToMovement()
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

    
    private IEnumerator Grow()
    {
        isInCoroutine = true;
        float tongueLen = (tongueEndPoint - new Vector2(tongueStartPointRef.position.x, tongueStartPointRef.position.y)).magnitude;
        Debug.Log(tongueLen);
        //Calculates how long it will take to shoot tongue based on distance.
        //Closer something is, less time it will take to shoot tongue.
        float modTongueShootTime = baseTongueShootTime * (tongueLen/10);

        Vector3 startScale = tongueBridge.transform.localScale;
        Vector3 maxScale = new Vector3(tongueLen + 0.1f, tongueWidth, 1f);

        do
        {
            if (tongueBridge != null)
            {
                tongueBridge.transform.localScale = Vector3.Lerp(startScale, maxScale, growthTimer / modTongueShootTime);
                growthTimer += Time.deltaTime;
                yield return null;
            }
            else
            {
                break;
            }
            
        }
        while (growthTimer <= modTongueShootTime);

        tongueOut = true;
        isInCoroutine = false;
    }

    private IEnumerator Degrow()
    {
        isInCoroutine = true;
        growthTimer = 0f;
        float tongueLen = (tongueEndPoint - new Vector2(tongueStartPointRef.position.x, tongueStartPointRef.position.y)).magnitude;
        //Calculates how long it will take to shoot tongue based on distance.
        //Closer something is, less time it will take to shoot tongue.
        float modTongueShootTime = baseTongueShootTime * (tongueLen / 10);

        Vector3 startScale = tongueBridge.transform.localScale;
        
        Vector3 minScale = new Vector3(0, 0.1f, 1);

        do
        {
            if (tongueBridge != null)
            {
                tongueBridge.transform.localScale = Vector3.Lerp(startScale , minScale, growthTimer / modTongueShootTime);
                growthTimer += Time.deltaTime;
                yield return null;
            }
            else
            {
                break;
            }

        }
        while (growthTimer <= modTongueShootTime);

        despawnTongueBridge();
        Uncrouch();
        isInCoroutine = false;
    }

    public override void ResetCharacter()
    {
        if (tongueOut)
        {
            ReturnInputBackToMovement();
            despawnTongueBridge();
            Uncrouch();
        }

        if (isAiming)
        {
            ReturnInputBackToMovement();
            Uncrouch();
            isAiming = false;
            aimingSprites.SetActive(false);
            crosshairSprite.SetActive(false);

            // Makes sure aiming pointer is not rotating on start of next aiming
            aimingRotationInput = 0;

            aimingSprites.transform.parent = tongueStartPointRef.transform;
            // Reset the aiming sprite to point upwards again
            aimingSprites.transform.rotation = new Quaternion(0, 0, 0, 0);
        }

        base.ResetCharacter();
    }

#endregion
}
