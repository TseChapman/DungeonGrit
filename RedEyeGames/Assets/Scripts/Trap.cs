﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{

    [SerializeField] private int trapDamage = 10;
    [SerializeField] private float trapKnockbackForce = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            collision.gameObject.GetComponent<Health>().TakeDamage(trapDamage, trapKnockbackForce, this.transform);
    }
}