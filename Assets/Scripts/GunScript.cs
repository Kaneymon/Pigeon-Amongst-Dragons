using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunScript : MonoBehaviour
{
    /*gun shooting Types:
     * 
     * single fire
     * full auto
     * burst
     * 
     * gun bullet types:
     * buckshot         
     * laser            
     * low caliber      
     * high caliber     
     * 
     * projectile
     * particles
     * 
     * funky buffs:
     * slow reload - high dmg, no extra ammo cost
     * blow your load - shoot all remaining bullets very very quickly
     * noBLUUm - no recoil at all.
     * 
     * gun stats:
     * dmg per shot
     * ammo per shot
     * time between shots
     * reload time
     * fire mode
     * bullet type
     * funky modifiers.
     * 
     * */

    // additional features:
    // tracers -- colour -- thickness -- time
    // shot impacts -- particle system -- image decal
    // screen flash -- png


    //Guns Stats:
    [Header("Guns Stats")]
    [SerializeField] float dmgPerBullet;
    [SerializeField] int ammoPerShot;
    [SerializeField] float fireRate;
    [SerializeField] float reloadTime;
    [SerializeField] int magSize;
    [SerializeField] float RecoilForce;
    [SerializeField] float bulletSpread = 0.1f;
    [SerializeField] float bulletForce;

    //the current projectile being shot out the gun. if any.
    [SerializeField] GameObject projectileObject;

    //the current bullet Mode
    [Header("Bullet Mode")]
    
    [SerializeField] BulletMode bulletType;
    enum BulletMode { raycast = 0, projectile = 1, particle = 2 }

    //Guns Sound Effects and sound Players
    [Header("Sound")]
    [SerializeField] AudioSource gunAudio;
    [SerializeField] AudioClip ShootSound;
    [SerializeField] AudioClip reloadSound;

    [Header("VFX Objects")]
    [SerializeField] ParticleSystem muzzleFlash;

    //object references for bullet origin and direction
    [Header("bullet spawning obj's")]
    [SerializeField] GameObject shotOrigin;
    [SerializeField] GameObject shotDirectionObj; //would be the players camera in a fps
    [SerializeField] GameObject Player;

    //weapon state system stuff.
    private bool reloading = false;
    [SerializeField] private int ammoLeft;

    [Header("Joose")]
    [SerializeField] UnityEvent Joose;


    //-----Getters And Setters-----
    public float GetBulletDamage(){return dmgPerBullet;}
    public void SetBulletDamage(float newDamageVal) { dmgPerBullet = newDamageVal; }


    public float GetFireRate() { return fireRate; }
    public void SetFireRate(float newFireRate) { fireRate = newFireRate; }

    public int GetMagSize() { return magSize; }
    public void SetMagSize(int newMagSize) { magSize = newMagSize; }


    public float GetReloadTime() { return reloadTime; }
    public void SetReloadTime(float newReloadTime) { reloadTime = newReloadTime; }




    void GunEffects()
    {
        //play audio
        gunAudio.PlayOneShot(ShootSound);
        //run particle system
        if (muzzleFlash)
        {
            muzzleFlash.Play();
        }

        // add body recoil
        Player.GetComponent<Rigidbody>().AddForce(-shotOrigin.transform.forward * RecoilForce * ammoPerShot, ForceMode.Impulse);
    }



    //-----Gun Functionality Functions-----
    Quaternion gunRotation;
    void ShootProjectiles()//create an alternate version for raycasted bullets
    {
        if (reloading){ gunAudio.PlayOneShot(reloadSound); return;}

        GunEffects();

        //repeat these two steps "ammoPerShot" times
        for (int i = 0; i < BulletsToShoot(); i++)
        {
            //instantiate projectile obj
            gunRotation = this.transform.rotation;
            GameObject newBullet = Instantiate(projectileObject, shotOrigin.transform.position, Quaternion.Euler(new Vector3 (shotDirectionObj.transform.rotation.eulerAngles.x-90, shotDirectionObj.transform.rotation.eulerAngles.y, shotDirectionObj.transform.rotation.eulerAngles.z)));
            //add force to projectile
            newBullet.GetComponent<Rigidbody>().AddForce(shotDirectionObj.transform.forward * bulletForce, ForceMode.Impulse);
            Joose.Invoke();
        }
        ammoLeft -= BulletsToShoot();
    }



    LayerMask raycastLayerMask;
    void ShootRaycasts()
    {
        if (reloading){ gunAudio.PlayOneShot(reloadSound); return; }

        GunEffects();

        //repeat these two steps "ammoPerShot" times
        for (int i = 0; i < BulletsToShoot(); i++)
        {

            gunRotation = this.transform.rotation;
            RaycastHit hit;

            if (Physics.Raycast(shotOrigin.transform.position, shotOrigin.transform.forward, out hit, 300f, raycastLayerMask))
            {
                Debug.Log(hit.transform.name);
                if (hit.transform.CompareTag("Enemy"))
                {
                    //EnemyController collisionEnemy = hit.transform.GetComponent<EnemyController>();
                    //collisionEnemy.SetEnemiesHealth(collisionEnemy.GetEnemiesHealth() - dmgPerBullet);
                }
            }
            Joose.Invoke();
        }
        ammoLeft -= BulletsToShoot();
    }


    void ShootParticles()
    {
        if (reloading) { gunAudio.PlayOneShot(reloadSound); return; }

        GunEffects();

        //repeat these two steps "ammoPerShot" times
        for (int i = 0; i < BulletsToShoot(); i++)
        {

            gunRotation = this.transform.rotation;
            

            
            Joose.Invoke();
        }
        ammoLeft -= BulletsToShoot();
    }

    //create intervals between our gun shots
    private float shotTimer;
    void ShootingManager()
    {
        shotTimer -= Time.deltaTime;

        if (shotTimer <= 0)
        {
            if (bulletType == BulletMode.projectile)
            {
                ShootProjectiles();
            }
            else if (bulletType == BulletMode.raycast)
            {
                ShootRaycasts();
            }
            else if (bulletType == BulletMode.particle)
            {
                ShootParticles();
            }
            shotTimer = 1 / fireRate;
        }
    }

    private int BulletsToShoot()
    {
        int ammo;
        if (ammoLeft < ammoPerShot)
        {
            ammo = ammoLeft;
        }
        else if (ammoLeft >= ammoPerShot)
        {
            ammo = ammoPerShot;
        }
        else
        {
            ammo = 0;
        }

        return ammo;
    }


    float reloadTimer;
    void ReloadGun()
    {
        reloadTimer -= Time.deltaTime;

        if (reloadTimer <=0)
        {
            ammoLeft = magSize;
            reloadTimer = reloadTime;
            reloading = false;
            gunAudio.PlayOneShot(reloadSound);
        }
    }

    private void Update()
    {
        if (ammoLeft == 0)
        {
            reloading = true;
        }
        if (reloading)
        {
            ReloadGun();
        }
        //whatever condition needs to be met for shooting to continue
        if (Input.GetKey(KeyCode.Mouse0))
        {
            //shoot the GUN!!!
            ShootingManager();
            print("shootin");
        }

    }

    private void Start()
    {
        shotTimer = 1 / fireRate;
        ammoLeft = magSize;
        reloadTimer = reloadTime;
        raycastLayerMask = LayerMask.GetMask("shootable_Raycast");
    }

}
