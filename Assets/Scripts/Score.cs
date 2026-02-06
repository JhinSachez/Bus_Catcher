using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private int monedas;

    [SerializeField] Text monedatexto;
    // Start is called before the first frame update
    void Start()
    {
        monedas = 0;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Moneda")
        {
            monedas++;
            monedatexto.text = "= " + monedas;
        }
        if (col.gameObject.tag == "Item")
        {
            monedas += 10;
            monedatexto.text = "= " + monedas;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
