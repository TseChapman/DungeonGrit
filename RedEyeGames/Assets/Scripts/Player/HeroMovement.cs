﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMovement : MonoBehaviour
{
    private bool jump = false;

    private Health health;

    [SerializeField] private HeroController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private float  walkSpeed = 15f;
    [SerializeField] private float runSpeedMultiplier = 2f;
    [SerializeField] private float horizontalMove = 0f;
    private Renderer heroRenderer;
    private int swordColor = 1;  


    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        heroRenderer = this.GetComponent<Renderer>();
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
    }

    public void OnLanding() { animator.SetBool("IsJumping", false); }

    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
}
