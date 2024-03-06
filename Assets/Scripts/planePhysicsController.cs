using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planePhysicsController : MonoBehaviour
{
    // Start is called before the first frame update

    Rigidbody rb;
    public Transform characterBody;
    [Header("thrust and lift variables")]
    public AnimationCurve liftCoef;
    public float liftDragCoef;
    public float dragCoef;
    public float thrustForce = 10;
    public float flapForce = 50;

    [Header("viewable variables")]
    public float thrustDrag;
    public float angleOfAttack;
    public Vector3 lift;

    [Header("pitch and roll variables")]
    public float pitchTorque = 1;
    public float rollTorque = 1;
    public float turnTorque = 1;

    [Header("Input Flight")]
     KeyCode Flap = KeyCode.Space;
     KeyCode fly = KeyCode.LeftShift;
     KeyCode bomb = KeyCode.Mouse0;
     KeyCode yawLeft = KeyCode.Q;
     KeyCode yawRight = KeyCode.E;
     KeyCode rollLeft = KeyCode.A;
     KeyCode rollRight = KeyCode.D;
     KeyCode pitchUp = KeyCode.W;
     KeyCode pitchDown = KeyCode.S;

    [Header("Input Ground")]
    KeyCode leftStrafe = KeyCode.A;
    KeyCode rightStrafe = KeyCode.D;
    KeyCode forwards = KeyCode.W;
    KeyCode backwards = KeyCode.S;

    [Header("grounded Variables")]
    public float moveSpeedGround = 8;

    [Header("animation")]
    [SerializeField] Animator anim;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        GroundChecking();
        SpeedControl();

        if (grounded)
        {
            WalkingMovement();
        }
        else
        {
            myflightInput();
            PitchRollTurn();
            LiftAndThrust();
            AngleOfAttack();
        }
    }


    private void LiftAndThrust()
    {
        
        Vector3 moveDirection = characterBody.forward;
        float liftForce = ((liftCoef.Evaluate(angleOfAttack) * Mathf.Pow(rb.velocity.z, 2)) / 2) - (liftCoef.Evaluate(angleOfAttack) * liftDragCoef * Mathf.Pow(rb.velocity.y, 2)) / 2;
        lift = characterBody.up * liftForce;

        thrustDrag = ((dragCoef * Mathf.Pow(rb.velocity.z, 2)) / 2);


        rb.AddForce((moveDirection * thrustForce) - (thrustDrag * moveDirection), ForceMode.Force);                  //forward backward forces
        rb.AddForce(lift, ForceMode.Acceleration);   //up down forces
    }


    private void FlapWings(){

        Vector3 moveDirection = characterBody.forward;
        rb.AddForce((moveDirection * flapForce), ForceMode.Impulse);

    }

    private void AngleOfAttack()
    {
        angleOfAttack = Vector3.Angle(transform.forward, rb.velocity.normalized);

        print(angleOfAttack);
    }

    private void PitchRollTurn()
    {
        if (Input.GetKey(pitchUp))
        {
            rb.AddTorque(characterBody.right * pitchTorque, ForceMode.Force);
        }
        if (Input.GetKey(pitchDown))
        {
            rb.AddTorque(characterBody.right * -pitchTorque, ForceMode.Force);
        }

        if (Input.GetKey(yawLeft))
        {
            rb.AddTorque(characterBody.up * -turnTorque * 0.5f, ForceMode.Force);
        }
        if (Input.GetKey(yawRight))
        {
            rb.AddTorque(characterBody.up * turnTorque * 0.5f, ForceMode.Force);
        }


        if (Input.GetKey(rollLeft))
        {
            rb.AddTorque(characterBody.forward * rollTorque, ForceMode.Force);
        }
        if (Input.GetKey(rollRight))
        {
            rb.AddTorque(characterBody.forward * -rollTorque, ForceMode.Force);
        }



        if (!Input.GetKeyDown(pitchUp))
        {
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void myflightInput() 
    {
        if (Input.GetKeyDown(Flap))
        {
            FlapWings();
            anim.SetBool("isFlapping", true);
        }
        if (Input.GetKeyUp(Flap))
        {
            FlapWings();
            anim.SetBool("isFlapping", false);
        }
        if (Input.GetKeyDown(bomb))
        {
            Bomb();
        }
    }

    private void Bomb()
    {
        
    }

    private void SpeedControl() //just because i kept hitting walls and launcing into space.  may be able to remove if i just add the walking controller.
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > thrustForce)
        {
            Vector3 limitedVel = flatVel.normalized * thrustForce;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }


    //-----------------------------------------
    //walking related functions

    float verticalInput = 0;
    float horizontalInput = 0;
    private void WalkingMovement()
    {

        //get movement input
        if (Input.GetKey(forwards)) { verticalInput = 1; }
        else if (Input.GetKey(backwards)) { verticalInput = -1; }
        else { verticalInput = 0; }

        if (Input.GetKey(rightStrafe)) { horizontalInput = 1; }
        else if (Input.GetKey(leftStrafe)) { horizontalInput = -1; }
        else { horizontalInput = 0; }

        // calculate movement direction
        Vector3 moveDirection = characterBody.forward * verticalInput + characterBody.right * horizontalInput;

        //apply movement direction to character.
        rb.AddForce(moveDirection.normalized * moveSpeedGround * 10f, ForceMode.Force);
    }

    [Header("groundCheck")]
    public bool grounded = false;
    public float playerHeight = 1;
    public LayerMask whatIsGround;
    private void GroundChecking()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
    }


}
