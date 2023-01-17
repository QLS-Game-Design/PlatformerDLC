using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Start is called before the first frame update
    public float damageBullet;
    public float damageGrenade;
    public float damageBeam;
    public GameObject player;
    //Vector2 mousePos;
    public Camera mainCam;
    public Vector2 camera;
    // public Transform firePoint = this;
    public GameObject bulletPrefab;
    public GameObject grenadePrefab;
    public GameObject beamPrefab;
    public GameObject shotgunPrefab;
    public GameObject muzzle;
    public bool isFlipped = false;
    public float weaponOffsetIdleX;
    public float weaponOffsetIdleY;
    public float weaponOffsetRunningX;
    public float weaponOffsetRunningY;
    public float aimThreshold = 1;
    public bool shootingBurstOn = false;
    private float timeFromLastShot = 0f;
    public float shotWaitTime = 0.2f;
    public bool grenadeOn;
    public bool gunOn = false;
    public bool beamBurst = false;
    Sprite weaponSprite;
    public Sprite gunSprite;
    public Sprite grenadeSprite;
    public Sprite beamSprite;
    public Sprite shotgunSprite;

    public AudioSource gunFireAudio;
    public AudioSource grenadeFireAudio;

    // enum Gun{
    //     pistol,
    //     grenade,
    //     beam,
    // }

    void Start()
    {
        weaponSprite = GetComponent<SpriteRenderer>().sprite;
        bulletPrefab.GetComponent<Bullet>().damage = StaticTracker.gunDamage;
        grenadePrefab.GetComponent<Grenade>().damage = StaticTracker.grenadeDamage;
        beamPrefab.GetComponent<Beam>().damage = StaticTracker.beamDamage;
        //shotgunPrefab.GetComponent<Shotgun>().damage = StaticTracker.beamDamage;
        gunFireAudio = GameObject.Find("GunFireAudio").GetComponent<AudioSource>();
        grenadeFireAudio = GameObject.Find("GrenadeFireAudio").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Shoot(int k){
        if (shootingBurstOn && weaponSprite == gunSprite){

            GameObject clone = Instantiate(bulletPrefab,muzzle.transform.position, muzzle.transform.rotation);
            Physics2D.IgnoreCollision(clone.GetComponent<Collider2D>(), this.GetComponentInParent<Collider2D>());
            clone.GetComponent<Bullet>().speed *= k;
            gunFireAudio.Play();
        } if(weaponSprite == grenadeSprite)  {
            GameObject clone = Instantiate(grenadePrefab,muzzle.transform.position, muzzle.transform.rotation);
            Physics2D.IgnoreCollision(clone.GetComponent<Collider2D>(), this.GetComponentInParent<Collider2D>());
            clone.GetComponent<Grenade>().speed *= k;
            grenadeFireAudio.Play();
        } if(weaponSprite == beamSprite && beamBurst){
            GameObject clone = Instantiate(beamPrefab,muzzle.transform.position, muzzle.transform.rotation);
            Physics2D.IgnoreCollision(clone.GetComponent<Collider2D>(), this.GetComponentInParent<Collider2D>());
            clone.GetComponent<Beam>().speed *= k;
        }
        if(weaponSprite == shotgunSprite){
            GameObject clone = Instantiate(shotgunPrefab,muzzle.transform.position, muzzle.transform.rotation);
            Physics2D.IgnoreCollision(clone.GetComponent<Collider2D>(), this.GetComponentInParent<Collider2D>());
            clone.GetComponent<Shotgun>().speed *= k;
        }
    }
    void Update()
    {
        if (!player.GetComponent<Player1>().onGround || player.GetComponent<Player1>().health <= 0 || player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Player_Hurt"))
        {
            GetComponent<SpriteRenderer>().sprite = null;
            return;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = weaponSprite;
        }

        float weaponOffsetX, weaponOffsetY;
        if (Mathf.Abs(player.GetComponent<Player1>().horizontalMove) > 0)
        {
            weaponOffsetX = weaponOffsetRunningX;
            weaponOffsetY = weaponOffsetRunningY;
        }
        else
        {
            weaponOffsetX = weaponOffsetIdleX;
            weaponOffsetY = weaponOffsetIdleY;
        }

        //mousePos = Input.mousePosition;
        camera = mainCam.ScreenToWorldPoint(Input.mousePosition);
        // Debug.Log(mousePos.x);
        float px = player.transform.position.x; // -this.transform.position.x
        float py = player.transform.position.y; //-this.transform.position.y
        float mx = camera.x-px;
        float my = camera.y-py;


        if (mx < 0){ // if the mouse is to the left of the character
            //GetComponent<SpriteRenderer>().flipX = true;
            //isFlipped = true;
            transform.position = new Vector3(px-weaponOffsetX, py+weaponOffsetY, -1);
            //player.GetComponent<SpriteRenderer>().flipX = true;
            
            Vector3 theScale = player.transform.localScale;
            if (theScale.x > 0)
            {
                theScale.x *= -1;
                player.transform.localScale = theScale;
            }
        } else {
            //GetComponent<SpriteRenderer>().flipX = false;
            //isFlipped = false;
            transform.position = new Vector3(px+weaponOffsetX, py+weaponOffsetY, -1);
            //player.GetComponent<SpriteRenderer>().flipX = false;

            Vector3 theScale = player.transform.localScale;
            if (theScale.x < 0)
            {
                theScale.x *= -1;
                player.transform.localScale = theScale;
            }
            
        }
        if (Input.GetButtonDown("Switch")){
            SwitchWeapon();
        }
        if (shootingBurstOn || beamBurst){
            if (Input.GetButton("Fire1")){
                if (timeFromLastShot > shotWaitTime){
                    if (mx < 0)
                        Shoot(-1);
                    else
                        Shoot(1);
                    timeFromLastShot = 0;
                } else {
                    timeFromLastShot += Time.deltaTime;
                }
            }
        } else {
            if (Input.GetButtonDown("Fire1")){
                if (mx < 0)
                    Shoot(-1);
                else
                    Shoot(1);
            }
        }
        float cy = camera.y-muzzle.transform.position.y;
        float cx = camera.x-muzzle.transform.position.x;
        float theta = Mathf.Atan(cy/cx)*(180/Mathf.PI);
        if(Mathf.Sqrt(Mathf.Pow(cy,2) + Mathf.Pow(cx,2)) > aimThreshold){
            this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y, theta);
        }
    }
    void SwitchWeapon(){
        
        if (shootingBurstOn == false && gunOn && grenadeOn == false){
            weaponSprite = gunSprite;
            grenadeOn = false;
            gunOn = false;  
            shotWaitTime = 0.2f;
            shootingBurstOn = true;
            beamBurst = false;
        } 
        else if(shootingBurstOn && grenadeOn == false && gunOn == false){
            weaponSprite = grenadeSprite;
            grenadeOn = true;
            shotWaitTime = 0.2f;
            shootingBurstOn = false;
            }
        else if(grenadeOn && shootingBurstOn == false && gunOn == false) {
            
            weaponSprite = beamSprite;
            grenadeOn = false;
            shotWaitTime = 0f;
            gunOn = true;
            shootingBurstOn = false;
            beamBurst = true;
            }
            
        }
        
    }
