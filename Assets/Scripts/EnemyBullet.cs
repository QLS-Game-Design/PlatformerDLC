using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 20f;
    public float rotateSpeed = 200f;
    public float damage;
    public bool isBoss;
    public bool isHoming;
    public Rigidbody2D rb;
    public GameObject impactEffect;
    public Transform target;
    Vector3 m_EulerAngleVelocity;

    // Start is called before the first frame update
    void Start()
    {
        if (isBoss)
        {
            rb.velocity = -transform.up * speed;
        }
        else if (!isHoming)
        {
            rb.velocity = transform.right * speed;
        }
        if (isHoming)
        {
            target = GameObject.Find("Player").transform;
        }
    }
    void FixedUpdate(){
        if (isBoss){
            transform.Rotate(0, 0, 10f);
        }
        if (isHoming)
        {
            Vector2 direction = (Vector2)target.position - rb.position;
            direction.Normalize();
            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            rb.angularVelocity = -rotateAmount * rotateSpeed;
            rb.velocity = transform.up * speed;
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        //if (col.gameObject.tag != "Enemy" && col.gameObject.tag != "EnemyBullet")
        //{
        //    //Debug.Log(col.gameObject.tag);
        //    Instantiate(impactEffect, transform.position, transform.rotation);
        //    GameObject.FindGameObjectWithTag("Player").GetComponent<Player1>().animator.SetBool("IsHurt", false);
        //    Destroy(this.gameObject);
        //}

        Instantiate(impactEffect, transform.position, transform.rotation);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player1>().animator.SetBool("IsHurt", false);
        Destroy(this.gameObject);
    }
}