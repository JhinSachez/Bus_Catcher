using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camion : MonoBehaviour
{
   
   
    Ground  velocity;
    private void Awake()
    {
        velocity = GameObject.Find("Ground").GetComponent<Ground>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (velocity.acelerar == false)
        {
            float realVelocity = velocity.speed / 2;
            Vector2 pos = transform.position;

            pos.x -= realVelocity * Time.fixedDeltaTime;

            if (velocity.speed <= 30)
            {
                pos.x = 6.80f;
            }
            if (velocity.speed >= 30)
            {
                pos.x = 4;
            }

            if (velocity.speed >= 40)
            {
                pos.x = 3;
            }

            if (velocity.speed >= 45)
            {
                pos.x = -10;
            }
            transform.position = pos;
            
        }
    }
}
