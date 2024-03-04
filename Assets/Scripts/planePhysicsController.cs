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
    public float liftDrag;
    public float angleOfAttack;

    [Header("pitch and roll variables")]
    public float pitchTorque = 1;
    public float rollTorque = 1;
    public float turnTorque = 1;

    [Header("Input")]
     KeyCode Flap = KeyCode.Space;
     KeyCode bomb = KeyCode.Mouse0;
     KeyCode yawLeft = KeyCode.Q;
     KeyCode yawRight = KeyCode.E;
     KeyCode rollLeft = KeyCode.A;
     KeyCode rollRight = KeyCode.D;
     KeyCode pitchUp = KeyCode.W;
     KeyCode pitchDown = KeyCode.S;

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
        myInput();
        PitchRollTurn();
    }

    private void FixedUpdate()
    {

        LiftAndThrust();
        AngleOfAttack();

    }

    private void LiftAndThrust()
    {

        Vector3 moveDirection = characterBody.forward;

        Vector3 lift = characterBody.up * ((liftCoef.Evaluate(angleOfAttack) * Mathf.Pow(rb.velocity.z, 2)) / 2);
        liftDrag = ((liftDragCoef * Mathf.Pow(rb.velocity.y, 2)) / 2);
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
        angleOfAttack = characterBody.localRotation.eulerAngles.x-60;

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

    private void myInput() 
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

}
