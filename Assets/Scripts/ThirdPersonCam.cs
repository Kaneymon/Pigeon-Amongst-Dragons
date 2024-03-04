using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
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
    public PlayerMovement movement;
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

        if (!movement.grounded)
        {
            isFlying = true;
            FlyingCam();
        }
        else
        {
            isFlying = false;
            WalkingCam();
        }
    }

    private void FlyingCam()
    {
        //rotate the player object so its Y axis is pointing at the cameras Z axis.
        //rotate the player so its z axis 

        Vector3 camXRot = transform.rotation.eulerAngles;
        float playerYaw =  80 + camXRot.x;
        float playerPitch = 0;
        

        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput != 0)
            playerPitch = Mathf.Lerp(playerObj.transform.rotation.eulerAngles.y, playerObj.transform.rotation.eulerAngles.y + 30 * horizontalInput, Time.deltaTime * rotationSpeed);

        playerObj.localEulerAngles = new Vector3 (playerYaw, playerPitch, playerObj.transform.localEulerAngles.z);
    }

    private void WalkingCam()
    {

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
