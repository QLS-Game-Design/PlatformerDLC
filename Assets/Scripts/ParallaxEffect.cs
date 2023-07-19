using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
     private float startingPos; 
    private float lengthOfSprite;  
    public float AmountOfParallax; 
    public Camera MainCamera; 
    // Start is called before the first frame update
    void Start()
    {
            startingPos = transform.position.x;    
            //lengthOfSprite = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
         Vector3 Position = MainCamera.transform.position;
            float Temp = Position.x * (1 - AmountOfParallax);
            float Distance = Position.x * AmountOfParallax;

            Vector3 NewPosition = new Vector3(startingPos + Distance, transform.position.y, transform.position.z);

            transform.position = NewPosition;
    }
}
