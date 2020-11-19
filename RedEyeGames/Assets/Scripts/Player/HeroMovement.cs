﻿﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroMovement : MonoBehaviour
{
    public GameObject camera;
    public GameObject background;
    private bool jump = false;
    private bool isStun = false;

    private Health health;

    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private HeroController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private float walkSpeed = 15f;
    [SerializeField] private float runSpeedMultiplier = 2f;
    [SerializeField] private float horizontalMove = 0f;

    // For Reset Purposes
    private float mInitSpeed;
    private float mInitSpeedMultiplier;
    private float mInitHorizontalMove;

    [SerializeField] private float stunDuration = 0.4f;

    [SerializeField] private float maxJump = 11.4f; // tested number to ensure player does not "super jump"

    public void SetSpeed(float speed)
    {
        walkSpeed = speed;
    }

    public float GetSpeed()
    {
        return walkSpeed;
    }

    public void ResetParameter()
    {
        walkSpeed = mInitSpeed;
        runSpeedMultiplier = mInitSpeedMultiplier;
        horizontalMove = mInitHorizontalMove;
    }

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        InitResetParameter();
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
        // ensure player does not "super jump"
        if (rigidBody2D.velocity.y > maxJump)
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, maxJump);

        if (isStun || health.IsDead())
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
    }

    private void UpdateCamera()
    {
        Vector3 position = camera.transform.position;
        position.x = gameObject.transform.position.x;
        position.y = gameObject.transform.position.y;
        camera.transform.position = position;
        position.z = 0;
        background.transform.position = position;
    }

    private void InitResetParameter()
    {
        mInitSpeed = walkSpeed;
        mInitSpeedMultiplier = runSpeedMultiplier;
        mInitHorizontalMove = horizontalMove;
    }

    public void OnLanding() { animator.SetBool("IsJumping", false); }

    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("OutBounds"))
        {
            health.Death();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            int damage = collision.gameObject.GetComponent<EnemyNormalBehavior>().CollisionDamage();
            float knockbackForce = collision.gameObject.GetComponent<EnemyNormalBehavior>().KnockbackForce();
            health.TakeDamage(damage, knockbackForce, collision.transform);
        }
    }

    public void Knockback(Transform obj, float knockbackForce)
    {
        if (obj.position.x - this.transform.position.x > 0)
        {
            rigidBody2D.AddForce(new Vector2(-4, 1) * knockbackForce, ForceMode2D.Impulse);
        }
        else
        {
            rigidBody2D.AddForce(new Vector2(4, 1) * knockbackForce, ForceMode2D.Impulse);
        }
    }

    public IEnumerator Stun()
    {
        isStun = true;
        horizontalMove = 0;
        yield return new WaitForSecondsRealtime(stunDuration);
        isStun = false;
    }
}
