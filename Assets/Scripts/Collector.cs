using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        iCollector collectible = col.GetComponent<iCollector>();
        if (collectible != null)
        {
            collectible.Collect();
        }
    }
}
