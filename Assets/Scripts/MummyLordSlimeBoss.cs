using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyLordSlimeBoss : MonoBehaviour
{
    private LayerMask playerLayer;
    public float maxHealth = 20f;
    public float health;
    public Vector3 target;
    public int stage;
    public float stageOneThreshold = 199f;
    public float stageTwoThreshold = 120f;
    public float stageThreeThreshold = 80f;
    public float stageFourThreshold = 40f;
    public GameObject player;
    public GameObject bulletPrefab;
    public GameObject healthBar;
    public GameObject bandagePrefab;
    public GameObject bandageClear;
    public float flySpeedX = 0.05f;
    private float centerX = 0;
    Vector3 currentEulerAngles;
    Quaternion currentRotation;
    private float timeFromLastShot = 0f;
    public float shotWaitTime = 0.5f;
    private float timeFromLastBandageAttack = 0f;
    public float bandageAttackWaitTime = 3f;
    private bool runningBandageAttack;
    private bool tracking;
    private bool attacking;
    private float attackTime;
    public float bandageAttackPause = 0.75f;
    public float smashAttackDuration = 1f;
    public float smashSpeed = 0.05f;
    private bool hitGround;
    public GameObject deathEffect;
    public GameObject levelLoader;
    public GameObject slimePrefab;
    public GameObject bigSlimePrefab;
    private AudioSource bossDeathAudio;
    private float timeFromLastSpawn = 0f;
    public float spawnWaitTime = 0.5f;
    public GameObject spawner;
    public float chaseSpeed = 0.2f;
    public float chaseSpeedStageTwo = 0.26f;

    private GameObject bandageClone;
    private bool throwingBandage;
    public float bandageThrowSpeed;
    private bool displayingBandageClear;
    private GameObject bandageClearClone;
    private bool flippedGravity;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        stage = 0;
        playerLayer = LayerMask.GetMask("Player");
        bossDeathAudio = GameObject.Find("BossDeathAudio").GetComponent<AudioSource>();
        target = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        healthBar.GetComponent<HealthBar>().updateValue(health / maxHealth);
        //if (runningSmashAttack)
        //{
        //    if (tracking)
        //    {
        //        target.x = player.transform.position.x;
        //        transform.position = Vector3.MoveTowards(transform.position, target, chaseSpeed);
        //        if (Mathf.Abs(transform.position.x - player.transform.position.x) < 0.1)
        //        {
        //            //Debug.Log(transform.position.x - player.transform.position.x);
        //            tracking = false;
        //            StartCoroutine(SmashAttackPause());
        //        }
        //    } else if (attacking)
        //    {
        //        if (!hitGround)
        //        {
        //            transform.position += Vector3.down * smashSpeed;
        //        }
        //        attackTime += Time.deltaTime;
        //    }
        //    if (attackTime > smashAttackDuration)
        //    {
        //        Debug.Log("stopping smash attack");
        //        timeFromLastSmash = 0f;
        //        tracking = false;
        //        attacking = false;
        //        attackTime = 0f;
        //        runningSmashAttack = false;
        //    }
        //    return;
        //}

        if (runningBandageAttack)
        {
            if (throwingBandage)
            {
                print("throwing bandage");
                bandageClone.transform.localScale += new Vector3(0, bandageThrowSpeed, 0);
                if (bandageClone.transform.localScale.y >= 100)
                {
                    throwingBandage = false;
                    displayingBandageClear = false;
                    runningBandageAttack = false;
                    GameObject.Destroy(bandageClone);
                    GameObject.Destroy(bandageClearClone);
                    Debug.Log("stopping bandage attack (and throwing bandage and displaying bandage clear)");
                    timeFromLastBandageAttack = 0f;
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                }
                return;
            }

            if (!displayingBandageClear)
            {
                player = GameObject.FindGameObjectWithTag("Player");
                float theta = Mathf.Atan((player.transform.position.x - transform.position.x) / (transform.position.y - player.transform.position.y)) * (180 / Mathf.PI);
                currentEulerAngles = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y, theta);
                currentRotation.eulerAngles = currentEulerAngles;
                transform.rotation = currentRotation;
                bandageClone = Instantiate(bandagePrefab, new Vector3(spawner.transform.position.x, spawner.transform.position.y, 10), transform.rotation);
                //runningBandageAttack = false;

                displayingBandageClear = true;
                StartCoroutine(BandageAttackPause());
                //throwingBandage = true;
            }


            return;
        }

        
        if (health<=stageOneThreshold){
            if (!runningBandageAttack && timeFromLastBandageAttack > bandageAttackWaitTime)
            {
                runningBandageAttack = true;
                Debug.Log("starting bandage attack");
            } 
            else {
                fly();
                timeFromLastBandageAttack += Time.deltaTime;
            }
        } 
        //else {
        //    target.x = transform.position.x;
        //}
        if (health <= stageTwoThreshold)
        {
            //this.GetComponent<SpriteRenderer>().color = Color.red; // make boss look angry
            //chaseSpeed = chaseSpeedStageTwo;
            print("Shooting down");
            ShootDown();
        }
        if (health<=stageThreeThreshold){
            SpawnSlimes(slimePrefab);
            //if(health <= stageFourThreshold){
            //    SpawnSlimes(bigSlimePrefab);
            //} else {
            //    SpawnSlimes(slimePrefab);
            //}
        }
        if (!flippedGravity && health <= stageFourThreshold)
        {
            Physics2D.gravity = new Vector2(0, 9.8f);
            player.transform.Rotate(0, 0, 180f);
            flippedGravity = true;
        }
    }

    IEnumerator BandageAttackPause()
    {
        print("displaying bandage clear");

        bandageClearClone = Instantiate(bandageClear, new Vector3(spawner.transform.position.x, spawner.transform.position.y, 10), transform.rotation);
        yield return new WaitForSeconds(bandageAttackPause);
        print("setting throwing bandage to true");
        // GameObject.Destroy(bandageClearObject);
        throwingBandage = true;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            levelLoader.GetComponent<LevelLoader>().isGameOver = true;
            StartCoroutine(Die());
        }
    }
    public void SpawnSlimes(GameObject enemyPrefab){
        if (timeFromLastSpawn > spawnWaitTime){
            Instantiate(enemyPrefab, spawner.transform.position, spawner.transform.rotation);
            timeFromLastSpawn = 0;
        } else{
            timeFromLastSpawn += Time.deltaTime;
        }
        
    }
    // private void OnDrawGizmos()
    // {
    //    Gizmos.DrawLine(spawner.transform.position, new Vector2(spawner.transform.position.x, spawner.transform.position.y - 1f));
    // }
    IEnumerator Die()
    {
        bossDeathAudio.Play();
        Instantiate(deathEffect, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.1f);
        GameObject.Destroy(gameObject);
    }
    private void fly(){
        transform.position = Vector3.MoveTowards(transform.position, target, chaseSpeed);
        target.x += flySpeedX;
    }
    private void ShootDown(){
        player = GameObject.FindGameObjectWithTag("Player");
        float theta = Mathf.Atan((player.transform.position.x-transform.position.x)/(transform.position.y-player.transform.position.y))*(180/Mathf.PI);
        currentEulerAngles = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y, theta);
        currentRotation.eulerAngles = currentEulerAngles;
        transform.rotation = currentRotation;
        if (timeFromLastShot > shotWaitTime){
            GameObject clone = Instantiate(bulletPrefab, spawner.transform.position, spawner.transform.rotation);
            Physics2D.IgnoreCollision(clone.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            timeFromLastShot = 0;
        } else{
            timeFromLastShot += Time.deltaTime;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Bullet")
        {
            TakeDamage(collision.collider.gameObject.GetComponent<Bullet>().damage);
        }
        if (collision.collider.gameObject.tag == "Grenade")
        {
            TakeDamage(collision.collider.gameObject.GetComponent<Grenade>().damage);
        }
        if (collision.gameObject.tag == "Map")
        {
            hitGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Map")
        {
            hitGround = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Boundary")
        {
            flySpeedX *= -1;
        }
    }
}
