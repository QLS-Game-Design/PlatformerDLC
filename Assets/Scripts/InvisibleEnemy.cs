using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleEnemy : MonoBehaviour
{
    public float changeSpeed;

    SpriteRenderer spriteRenderer;

    int direction = -1;
    Color spriteColor;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteColor = spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        print(spriteColor.a);
        if (spriteColor.a <= 0)
        {
            direction = 1;
        } else if (spriteColor.a >= 1)
        {
            direction = -1;
        }
        spriteColor.a += direction * Time.deltaTime * changeSpeed;
        spriteRenderer.color = spriteColor;
    }
}
