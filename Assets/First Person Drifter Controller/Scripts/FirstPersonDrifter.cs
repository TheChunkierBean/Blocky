// original by Eric Haines (Eric5h5)
// adapted by @torahhorse
// http://wiki.unity3d.com/index.php/FPSWalkerEnhanced

using UnityEngine;
using System.Collections;
using BeardedManStudios.Network;

[RequireComponent (typeof (CharacterController))]
public class FirstPersonDrifter: NetworkedMonoBehavior
{
    public float walkSpeed = 6.0f;
    public float runSpeed = 10.0f;
 
    // If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
    private bool limitDiagonalSpeed = true;
 
    public bool enableRunning = false;
 
    public float jumpSpeed = 4.0f;
    public float gravity = 10.0f;
 
    // Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
    private float fallingDamageThreshold = 10.0f;
 
    // If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
    public bool slideWhenOverSlopeLimit = false;
 
    // If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
    public bool slideOnTaggedObjects = false;
 
    public float slideSpeed = 5.0f;
 
    // If checked, then the player can change direction while in the air
    public bool airControl = true;
 
    // Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
    public float antiBumpFactor = .75f;
 
    // Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
    public int antiBunnyHopFactor = 10;
 
    private Vector3 moveDirection = Vector3.zero;
    private bool grounded = false;
    private CharacterController controller;
    private Transform myTransform;
    private float speed;
    private RaycastHit hit;
    private float fallStartLevel;
    private bool falling;
    public Animator animator;
    private float slideLimit;
    private float rayDistance;
    private Vector3 contactPoint;
    private bool playerControl = false;
    private int jumpTimer;
    private float inputX = 0;
    private int angle = 90;
    private float inputY = 0;
    private Vector3 move;
    private bool jump;
    private void Awake()
    {
        AddNetworkVariable(() => moveDirection, x => moveDirection = (Vector3)x);
        AddNetworkVariable(() => grounded, x => grounded = (bool)x);
        AddNetworkVariable(() => angle, x => angle = (int)x);
        AddNetworkVariable(() => inputX, x => inputX = (float)x);
        AddNetworkVariable(() => inputY, x => inputY = (float)x);
        AddNetworkVariable(() => jump, x => jump = (bool)x);
        AddNetworkVariable(() => contactPoint, x => contactPoint = (Vector3)x);
        AddNetworkVariable(() => playerControl, x => playerControl = (bool)x);
    }
    protected override void NetworkStart()
    {
        base.NetworkStart();

        if (IsOwner)
        {
            GetComponentInChildren<Camera>().enabled = true;
            GetComponentInChildren<AudioListener>().enabled = true;
            foreach (Transform child in transform)
            {
                if (child.name == "advancedCharacter")
                {
                    DestroyObject(child.gameObject);
                }
            }
        }
        else
        {
            GetComponentInChildren<AudioListener>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
        }
        //GetComponent<CharacterController>().enabled = true;
    }
    void Start()
    {
        controller = GetComponent<CharacterController>();
        myTransform = transform;
        speed = walkSpeed;
        rayDistance = controller.height * .5f + controller.radius;
        slideLimit = controller.slopeLimit - .1f;
        jumpTimer = antiBunnyHopFactor;
    }
    void OnTriggerEnter(Collider obj)
    {
        Debug.Log("START: " + obj.transform.name);
    }
    public Transform mainCamera;
    void OnTriggerStay(Collider obj)
    {
       if(Input.GetKeyDown(KeyCode.G))
        {
            if (obj.transform.tag == "Weapon")
            {
                gameObject.GetComponentInChildren<GunSwitchScript>().PickUpGun(obj.transform, mainCamera);
            }
        }
    }
    void OnTriggerExit(Collider obj)
    {
        Debug.Log("EXIT: " + obj.transform.name);
    }
    void FixedUpdate() {
        if (!IsOwner)
        {
            animator.SetBool("Jump", jump);
            animator.SetFloat("MoveSpeed", inputY);
            animator.SetFloat("MoveDirection", angle);
        }
        else
        {
            inputX = Input.GetAxis("Horizontal");
            inputY = Input.GetAxis("Vertical");
        }
            angle = (int)Mathf.Round((Mathf.Atan(inputY / inputX) * (180 / Mathf.PI)));
            if (inputX < 0)
                angle = 180 + angle;
            if (angle < 0)
                angle = 360 + angle;
            Debug.Log(inputX +", " + inputY +", " + angle);
            // If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
            float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed)? .7071f : 1.0f;
 
            if (grounded) {
                bool sliding = false;
                // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
                // because that interferes with step climbing amongst other annoyances
                if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance)) {
                    if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                        sliding = true;
                }
                // However, just raycasting straight down from the center can fail when on steep slopes
                // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
                else {
                    Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                    if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                        sliding = true;
                }
 
                // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
                if (falling) {
                    falling = false;
                    if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
                        FallingDamageAlert (fallStartLevel - myTransform.position.y);
                }
                    
                if( enableRunning && IsOwner)
                {
            	    speed = Input.GetButton("Run")? runSpeed : walkSpeed;
                }
 
                // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
                if ( (sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide") ) {
                    Vector3 hitNormal = hit.normal;
                    moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                    Vector3.OrthoNormalize (ref hitNormal, ref moveDirection);
                    moveDirection *= slideSpeed;
                    playerControl = false;
                }
                // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
                else {
                    moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
                    moveDirection = myTransform.TransformDirection(moveDirection) * speed;
                    playerControl = true;
                }

            // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
            bool jumpCheck = false;
            if (IsOwner)
            {
                jumpCheck = Input.GetButton("Jump");
            }
            if (!jumpCheck)
            {
                jump = false;
                jumpTimer++;
            }
            else if (jumpTimer >= antiBunnyHopFactor)
            {
                jump = true;
                moveDirection.y = jumpSpeed;
                jumpTimer = 0;
            }
        }
            else {
                // If we stepped over a cliff or something, set the height at which we started falling
                if (!falling) {
                    falling = true;
                    fallStartLevel = myTransform.position.y;
                }
 
                // If air control is allowed, check movement but don't touch the y component
                if (airControl && playerControl) {
                    moveDirection.x = inputX * speed * inputModifyFactor;
                    moveDirection.z = inputY * speed * inputModifyFactor;
                    moveDirection = myTransform.TransformDirection(moveDirection);
                }
            }
            
            
            // Apply gravity
            moveDirection.y -= gravity * Time.deltaTime;
            // Move the controller, and set grounded true or false depending on whether we're standing on something
            grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
    }
 
    // Store point that we're in contact with for use in FixedUpdate if needed
    void OnControllerColliderHit (ControllerColliderHit hit) {
        contactPoint = hit.point;
    }
 
    // If falling damage occured, this is the place to do something about it. You can make the player
    // have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
    void FallingDamageAlert (float fallDistance)
    {
        //print ("Ouch! Fell " + fallDistance + " units!");   
    }
    public void Teleport(Vector3 loc)
    {
        
    
    }
}