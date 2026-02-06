using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Coin : MonoBehaviour, iCollector
{
    public Ground velocidad;
    public static event Action OnCoinCollected;
    private Rigidbody2D rb;
    private bool hasTarget;
    private Vector3 Targetposition;
    public float moveSpeed = 5;

    public void Start()
    {
        velocidad = GameObject.Find("Ground").GetComponent<Ground>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Collect()
    {
        Destroy(gameObject);
        OnCoinCollected?.Invoke();
    }

    private void FixedUpdate()
    {
        if (hasTarget)
        {
            Vector2 targetDirection = (Targetposition - transform.position).normalized;
            rb.linearVelocity = new Vector2(targetDirection.x, targetDirection.y) * moveSpeed;
        }
    }

    public void SetTarget(Vector3 position)
    {
        Targetposition = position;
        hasTarget = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            velocidad.speed += 1;
            Destroy(gameObject);
        }
    }
}
