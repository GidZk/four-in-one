using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RandomMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Random.value < 0.10)
        {
            rb.AddForce(Random.insideUnitCircle * (float) (0.1 + Random.value) * 20);
        }
    }
}