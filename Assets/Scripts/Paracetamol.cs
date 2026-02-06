using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paracetamol : MonoBehaviour
{
    private Ground velocidad;

    private void Start()
    {
        velocidad = GameObject.Find("Ground").GetComponent<Ground>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            velocidad.speed += 10;
            Destroy(gameObject);
        }
    }
    
    
}
