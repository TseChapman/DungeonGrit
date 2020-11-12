﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroMovement : MonoBehaviour
{
    private bool jump = false;

    private Health health;
    private Rigidbody2D rigidBody2D;

    [SerializeField] private HeroController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private float walkSpeed = 15f;
    [SerializeField] private float runSpeedMultiplier = 2f;
    [SerializeField] private float horizontalMove = 0f;

    [SerializeField] private float knockbackPower = 5f;


    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health.IsDead())
            return;
        
        horizontalMove = Input.GetAxisRaw("Horizontal") * walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            horizontalMove = horizontalMove * runSpeedMultiplier;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("IsJumping", true);
        }
<<<<<<< Updated upstream
        if (Input.GetButtonDown("Fire1"))
            animator.SetBool("IsAttacking", true);
        else
            animator.SetBool("IsAttacking", false);
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (swordColor == 4)
            {
                swordColor = 1;
            } else
            {
                swordColor++;
            }
        }
        if (swordColor == 1) {
            Debug.Log("blue");
            heroRenderer.material.SetColor("_Color", Color.blue);
        } else if (swordColor == 2) {
            Debug.Log("yellow");
            heroRenderer.material.SetColor("_Color", Color.yellow);
        } else if (swordColor == 3) {
            Debug.Log("green");
            heroRenderer.material.SetColor("_Color", Color.green);
        } else if (swordColor == 4) {
            Debug.Log("red");
            heroRenderer.material.SetColor("_Color", Color.red);
        }
=======
>>>>>>> Stashed changes
    }

    public void OnLanding() { animator.SetBool("IsJumping", false); }

    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Knockback(collision.transform);
        }
    }

    private void Knockback(Transform obj)
    {
        Vector2 direction = (obj.position - this.transform.position).normalized;
        if (direction.x >= 0)
        {
            rigidBody2D.AddForce(new Vector2(-2, 0.5f) * knockbackPower, ForceMode2D.Impulse);
        }
        else
        {
            rigidBody2D.AddForce(new Vector2(2, 0.5f) * knockbackPower, ForceMode2D.Impulse);
        }
    }
}
