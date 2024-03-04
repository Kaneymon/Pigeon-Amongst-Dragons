using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float flySpeed;

    [Header("FlightForces")]
    public AnimationCurve aoaLiftCurve;
    public AnimationCurve aoaThrustCurve;
    public float liftCoef;
    public float dragCoef;



    public float groundDrag;
    public float jumpForce;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode Glide = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    public Transform orientation;
    public Transform playerObj;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);


        // handle drag
        if (false)
        {
            MyInput();
            SpeedControl();
            rb.drag = groundDrag;
            WalkingMovement();
        }
        else
        {
            MyInput();
            SpeedControl();
            rb.drag = 0.05f;
            FlyingMovement();
        }
    }


    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump)
        {
            readyToJump = false;

            Jump();
            Invoke("ResetJump", 0.05f);
        }

    }



    private void WalkingMovement()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void FlyingMovement()
    {
        //reduce gravity
        if (verticalInput == 0)
        {
            verticalInput = 1;
        }

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;


        var aoa = AngleOfAttackFromTransform(playerObj);
        Vector3 lift = orientation.up * ((liftCoef * Mathf.Pow(rb.velocity.z, 2)) / 2);
        float drag = ((dragCoef * Mathf.Pow(rb.velocity.z, 2)) / 2);


        rb.AddForce((moveDirection * flySpeed * 10f) - (drag * orientation.forward), ForceMode.Force);                  //forward backward forces
        rb.AddForce(lift , ForceMode.Force);   //up down forces

    }

    private float AngleOfAttackFromTransform(Transform t)
    {
        float aoa = t.eulerAngles.x;
        if (aoa > 360-90)
        {
            aoa = aoa - 360;
        }
        return -aoa;
    }

    private void SpeedControl()
    {
        float maxspeed = 0;
        if (grounded)
        {
            maxspeed = moveSpeed;
        }
        else
        {
            return;
        }

        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > maxspeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxspeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
}