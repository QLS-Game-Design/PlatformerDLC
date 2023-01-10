using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam: MonoBehaviour
{
    public float speed = 0f;
    public float damage = 1f;
    public Rigidbody2D rb;
    public GameObject impactEffect;

    public AudioSource beamHitAudio;

    // Start is called before the first frame update
    void Start()
    {
        beamHitAudio = GameObject.Find("GrenadeHitAudio").GetComponent<AudioSource>();
        rb.velocity = transform.right * speed;
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag != "Player" && col.gameObject.tag != "Beam")
        {
            beamHitAudio.Play();
            Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
            
    }
}

