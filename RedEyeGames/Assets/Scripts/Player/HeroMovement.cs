﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroMovement : MonoBehaviour
{
    public GameObject mCamera;
    public GameObject background;
    private bool jump = false;
    private bool isStun = false;
    [SerializeField] private bool speedPotion = false;

    private Health health;

    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private HeroController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private float walkSpeed = 15f;
    [SerializeField] private float runSpeedMultiplier = 2f;
    [SerializeField] private float horizontalMove = 0f;

    // For Reset Purposes
    private float mInitSpeed;
    private float mInitSpeedMultiplier;
    private float mInitHorizontalMove;

    [SerializeField] private float speedPotionMultiplier = 1.5f;

    [SerializeField] private float stunDuration = 0.4f;

    [SerializeField] private float maxJump = 11.4f; // tested number to ensure player does not "super jump"

    [SerializeField] private bool isRolling;
    [SerializeField] private float rollTime = 0.5f;
    [SerializeField] private float rollSpeed = 7f;
    [SerializeField] private float rollCooldown = 2f;
    private float lastRoll = 0;
    private float rollTimeLeft;
    private bool canJump = true;
    private bool canMove = true;

    
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
    
    public void SpeedBoost(bool set)
    {
        speedPotion = set;
    }

    // Start is called before the first frame update
    void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        InitResetParameter();
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

        if (speedPotion)
            horizontalMove = horizontalMove * speedPotionMultiplier;
            
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        if (Input.GetButtonDown("Jump"))
        {
            if (canJump)
            {
                jump = true;
                animator.SetBool("IsJumping", true);
            }
        }

        if (Input.GetButtonDown("Roll"))
        {
            if (Time.time > (lastRoll + rollCooldown) && !animator.GetBool("IsJumping"))
                Roll();
        }
        CheckRoll();
    }

    private void Roll()
    {
        animator.SetTrigger("Roll");
        isRolling = true;
        rollTimeLeft = rollTime;
        lastRoll = Time.time;
    }

    private void CheckRoll()
    {
        if (isRolling)
        {
            if (rollTimeLeft > 0)
            {
                boxCollider2D.enabled = false;
                if (transform.localScale.x > 0)
                    if (controller.getGrounded())
                    {
                        rigidBody2D.bodyType = RigidbodyType2D.Kinematic;
                        rigidBody2D.velocity = new Vector2(rollSpeed, 0);
                    }
                    else
                    {
                        rigidBody2D.bodyType = RigidbodyType2D.Dynamic;
                        rigidBody2D.velocity = new Vector2(rollSpeed, rigidBody2D.velocity.y);
                    }
                else
                    if (controller.getGrounded())
                    {
                        rigidBody2D.bodyType = RigidbodyType2D.Kinematic;
                        rigidBody2D.velocity = new Vector2(-rollSpeed, 0);
                    }
                    else
                    {
                        rigidBody2D.bodyType = RigidbodyType2D.Dynamic;
                        rigidBody2D.velocity = new Vector2(-rollSpeed, rigidBody2D.velocity.y);
                    }
                rollTimeLeft -= Time.deltaTime;
            }
            if (rollTimeLeft < 0)
            {
                boxCollider2D.enabled = true;
                rigidBody2D.bodyType = RigidbodyType2D.Dynamic;
                isRolling = false;
            }
        }
    }

    public void YesMoveJump()
    {
        canMove = true;
        canJump = true;
    }

    public void NoMoveJump()
    {

        canMove = false;
        canJump = false;
    }

    private void UpdateCamera()
    {
        Vector3 position = mCamera.transform.position;
        position.x = gameObject.transform.position.x;
        position.y = gameObject.transform.position.y;
        mCamera.transform.position = position;
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
        if (canMove)
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
        if (collision.collider.CompareTag("Enemy") || collision.collider.CompareTag("Undead"))
        {
            int damage = collision.gameObject.GetComponent<EnemyNormalBehavior>().CollisionDamage();
            float knockbackForce = collision.gameObject.GetComponent<EnemyNormalBehavior>().KnockbackForce();
            health.TakeDamage(damage, knockbackForce, collision.transform);
        }
        else if (collision.collider.CompareTag("Boss"))
        {
            int damage = collision.gameObject.GetComponent<BossBehavior>().CollisionDamage();
            float knockbackForce = collision.gameObject.GetComponent<BossBehavior>().KnockbackForce();
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
