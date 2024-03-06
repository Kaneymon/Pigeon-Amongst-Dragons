using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class birdCameraController : MonoBehaviour
{

    public Transform Bird;
    public Transform orientation;
    public planePhysicsController birdController;

    private void Update()
    {
        if (birdController.grounded == true)
        {
            WalkingCam();
        }
    }

    private void WalkingCam()
    {
        // rotate orientation
        Vector3 viewDir = Bird.position - new Vector3(transform.position.x, Bird.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;


        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;



        if (inputDir != Vector3.zero)
            Bird.forward = Vector3.Slerp(Bird.forward, inputDir.normalized, Time.deltaTime * 7);




    }
}
