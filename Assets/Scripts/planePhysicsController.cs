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
    [Header("bonusVariables")]
    public float slomoTime = 1;
    public float slomoTimeCooldown = 25;
    public float BoostStaminUsage = 25;
    public PlayerStats playerStats;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        GroundChecking();
        SpeedControl();
        myflightInput();
        PitchRollTurn();
        LiftAndThrust();
        AngleOfAttack();
        TriggerSloMo();

    }

    private bool sloMoLock = false;
    private void TriggerSloMo()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse1) || sloMoLock)
        {
            return;
        }

        sloMoLock = true;
        Time.timeScale = 0.25f;
        pitchTorque *= 2;
        rollTorque *= 2;
        turnTorque *= 2;
        Invoke("StopSlomo", slomoTime);
    }

    private void StopSlomo()
    {
        Time.timeScale = 1f;
        pitchTorque /= 2;
        rollTorque /= 2;
        turnTorque /= 2;
        Invoke("ResetSloMoLock", slomoTimeCooldown);
    }
    private void ResetSloMoLock()
    {
        sloMoLock = false;
    }

    private float currentSpeed = 0;
    private void LiftAndThrust()
    {
        //thrust
        Vector3 moveDirection = rb.transform.forward;
        thrustDrag = (dragCoef * Mathf.Pow(rb.velocity.z, 2) / 2);
        rb.AddForce((moveDirection * currentSpeed) - (thrustDrag * moveDirection), ForceMode.Force);
        
        //lift
        float liftForce = (liftCoef.Evaluate(angleOfAttack) * liftDragCoef * Mathf.Pow(rb.velocity.z, 2)) / 2;
        lift = rb.transform.up * (liftForce);
        rb.AddForce(lift, ForceMode.Force);   //up down forces
    }


    private void FlapWings(){
        if (playerStats.Stamina<=1)
        {
            return;
        }
        Vector3 moveDirection = characterBody.forward;
        rb.AddForce((moveDirection * currentSpeed * 4), ForceMode.Force);
        if (playerStats.TakeStamina(BoostStaminUsage * Time.deltaTime))
        {
            playerStats.TakeStamina(BoostStaminUsage * Time.deltaTime);
        }
    }

    private void AngleOfAttack()
    {
        angleOfAttack = Vector3.SignedAngle(rb.velocity.normalized, characterBody.transform.forward, Vector3.forward); 
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
        if (Input.GetKey(Flap))
        {
            FlapWings();
        }
    }


    private void SpeedControl() 
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > thrustForce)
        {
            Vector3 limitedVel = flatVel.normalized * thrustForce;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }


    [Header("groundCheck")]
    public bool grounded = false;
    public float playerHeight = 1;
    public LayerMask whatIsGround;
    private void GroundChecking()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
        if (grounded)
        {
            rb.drag = 4;
            currentSpeed = moveSpeedGround;
        }
        else
        {
            rb.drag = 1;
            currentSpeed = thrustForce;
        }
    }


}
