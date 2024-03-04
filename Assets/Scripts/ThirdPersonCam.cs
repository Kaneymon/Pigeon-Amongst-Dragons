using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform flightOrientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    public float rotationSpeed;

    public Transform combatLookAt;

    public GameObject thirdPersonCam;
    public GameObject combatCam;
    public GameObject topDownCam;

    public CameraStyle currentStyle;
    public bool isFlying = true;

    public enum CameraStyle
    {
        Basic,
        Combat,
        Topdown
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isFlying = true;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            isFlying = false;
        }

        if (isFlying)
        {
            FlyingCam();
        }
        else
        {
            WalkingCam();
        }
    }

    private void FlyingCam()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = 1;

        Vector3 viewDir = playerObj.position - new Vector3(transform.position.x, player.position.y, transform.position.z);//change the y to transform for wild stuff

        flightOrientation.forward = viewDir.normalized;
        Vector3 inputDir = flightOrientation.forward * verticalInput + flightOrientation.right * horizontalInput;
        playerObj.up = Vector3.Slerp(playerObj.up, inputDir.normalized, Time.deltaTime * rotationSpeed);




        float pitch = 0;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            pitch = -2f;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            pitch = -0.4f;
        }
        else if (Input.GetAxis("Mouse Y") != 0)
        {
            pitch = -(Input.GetAxis("Mouse Y"));
        }

        playerObj.transform.Rotate(pitch, 0, 0);


    }

    private void WalkingCam()
    {
        // switch styles
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCameraStyle(CameraStyle.Basic);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchCameraStyle(CameraStyle.Combat);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchCameraStyle(CameraStyle.Topdown);

        // rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // roate player object
        if (currentStyle == CameraStyle.Basic || currentStyle == CameraStyle.Topdown)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }

        else if (currentStyle == CameraStyle.Combat)
        {
            Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirToCombatLookAt.normalized;

            playerObj.forward = dirToCombatLookAt.normalized;
        }
    }



    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);
        topDownCam.SetActive(false);

        if (newStyle == CameraStyle.Basic) thirdPersonCam.SetActive(true);
        if (newStyle == CameraStyle.Combat) combatCam.SetActive(true);
        if (newStyle == CameraStyle.Topdown) topDownCam.SetActive(true);

        currentStyle = newStyle;
    }
}
