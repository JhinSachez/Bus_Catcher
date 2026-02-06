using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent<Coin>(out Coin coin))
        {
            coin.SetTarget(transform.parent.position);
        }
    }
    
}
