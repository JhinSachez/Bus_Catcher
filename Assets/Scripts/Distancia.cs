using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Distancia : MonoBehaviour
{
     Ground velocity;
     Text distance;

     private void Awake()
     {
         velocity = GameObject.Find("Ground").GetComponent<Ground>();
         distance = GameObject.Find("Distancia").GetComponent<Text>();
     }

     // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int distances = Mathf.FloorToInt(velocity.distance);
        distance.text = distances + " m";
    }
}
