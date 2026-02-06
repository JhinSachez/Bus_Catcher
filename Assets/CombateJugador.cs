using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombateJugador : MenuGameOver
{
    public event EventHandler MuerteJugador;
    
    
    
    public  void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            MuerteJugador?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
        }
    }

}

    
