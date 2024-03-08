using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("levelStat")]
    public float POWERLEVEL = 0;
    [Header("healthStat")]
    public float Health = 100;
    public float maxHealth = 100;
    public float healthRecoveryRate = 5;
    [Header("staminaStat")]
    public float Stamina = 100;
    public float maxStamina = 100;
    public float staminaRecoveryRate = 7;
    [Header("speedStats")]
    public float thrustForce = 100;
    public float thrustBoostMultiplier = 2;
    [Header("attackStats")]
    public float physicalAttackMultiplier = 1;
    public float projectileDamageMultiplier = 1;
    [Header("expStats")]
    public float totalExp = 0;


    void Start()
    {
        Stamina = maxStamina;
        Health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        healthCheck();
    }

    private void FixedUpdate()
    {
        StaminaRecovery();
    }

    private void StaminaRecovery()
    {
        if (Stamina <= maxStamina)
        {
            Stamina += staminaRecoveryRate * Time.fixedDeltaTime;
        }
    }

    public bool TakeStamina(float amount)
    {
        if(amount > Stamina) { return false; }
        Stamina -= amount;
        return true;
    }

    public void healthCheck()
    {
        if (Health <= 0)
        {
            //end the game
            //restart the level?
            //respawn?
        }
    }

    public void takeHealth(float amount)
    {
        Health -= amount;
    }
    public void addHealth(float amount)
    {
        Health -= amount;
    }
}
