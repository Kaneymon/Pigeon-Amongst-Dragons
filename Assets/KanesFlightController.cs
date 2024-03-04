using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KanesFlightController : MonoBehaviour
{
    //when gliding, downwards force has to match closely the upwards forces.

    Rigidbody rb;
    public float thrust;
    public float lift;
    public float turnTorque;
    public float rollTorque;

    private void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if(Input.GetKey(KeyCode.Space)){
            rb.AddForce(transform.up * rb.mass*lift, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * rb.mass*thrust, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddTorque(-Vector3.up * rb.mass * turnTorque, ForceMode.VelocityChange);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddTorque(Vector3.up * rb.mass * turnTorque, ForceMode.VelocityChange);
        }

        rb.AddTorque(Vector3.right * -Input.GetAxis("Mouse Y")*0.1f, ForceMode.VelocityChange);
        rb.AddTorque(Vector3.forward * Input.GetAxis("Mouse X")*0.1f, ForceMode.VelocityChange);
    }
}
