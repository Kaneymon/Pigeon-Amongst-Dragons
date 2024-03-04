using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeSoundsManager : MonoBehaviour
{
    [SerializeField] AudioSource windPlayer;
    [SerializeField] AudioClip wind;
    float maxSpeed;
    planePhysicsController planeControls;

    Rigidbody rb;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        planeControls = gameObject.GetComponent<planePhysicsController>();
        maxSpeed = planeControls.thrustForce;
    }

    // Update is called once per frame
    void Update()
    {
        windPlayer.volume = Mathf.Clamp( (rb.velocity.magnitude/maxSpeed) * 1, 0f, 1f);
        windPlayer.pitch = Mathf.Clamp(0.8f + (rb.velocity.magnitude/maxSpeed ) * 0.9f, 0.8f, 1.8f);
    }
}
