using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiHandler : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform targetingPoint;
    [SerializeField] planePhysicsController playerControls;
    [SerializeField] PlayerStats stats;
    [SerializeField] Camera uiCam;
    [SerializeField] Text velText;
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider staminaSlider;
    [SerializeField] Image crosshair;

    private void UpdateVelocityTextComponents()
    {
        velText.text = Mathf.Round(rb.velocity.magnitude * 100f) / 100f + " mph";

    }

    private void UpdateStaminaComponents()
    {
        staminaSlider.value = Mathf.Clamp01(stats.Stamina / stats.maxStamina);
    }

    private void UpdateHealthComponents()
    {
        healthSlider.value = Mathf.Clamp01( stats.Health / stats.maxHealth);
    }

    private void positionCrossHair()
    {
        //take character controller.
        //get its forward vector + z amount
        // project world the screenpos
        //place crosshair at this coordinate.
        Vector3 worldPos =  targetingPoint.position;
        Vector2 screenPos = uiCam.WorldToScreenPoint(worldPos);
        crosshair.rectTransform.anchoredPosition = screenPos - new Vector2(uiCam.scaledPixelWidth/2, uiCam.scaledPixelHeight/2);
    }

    private void FixedUpdate()
    {
        UpdateVelocityTextComponents();

    }

    private void Update()
    {
        positionCrossHair();
        UpdateStaminaComponents();
        UpdateHealthComponents();
    }
}
