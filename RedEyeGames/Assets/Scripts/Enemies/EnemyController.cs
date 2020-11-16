using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyBehavior { PATROL = 0, FLY = 1, TRACKING = 2, GUARD = 3, NUM_ENEMY_BEHAVIOR = 4 }

public class EnemyController : MonoBehaviour
{
    public Slider slider;

    public EnemyBehavior enemyBehavior;
    private Rigidbody2D rigidbody2D;
    private Canvas canvas;
    private SpriteRenderer renderer;
    private Animator animator;

    [SerializeField] private int initHealth = 0;
    [SerializeField] private int mCurrentHealth;
    [SerializeField] private float runSpeed = 0f;
    [SerializeField] private float attackRate = 0f;
    [SerializeField] private float attackRange = 0f;
    [SerializeField] private float trackingDistance = 0f; // Only used when enemyBehavior is Tracking

    [SerializeField] private bool isStun = false;
    [SerializeField] private float stunDuration = 0.5f;
    private bool isPoisoned = false;

    // Start is called before the first frame update
    private void Start()
    {
        mCurrentHealth = initHealth;
        rigidbody2D = GetComponent<Rigidbody2D>();
        canvas = GetComponentInChildren<Canvas>();
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        canvas.enabled = false;
        SetMaxHealth();
    }

    public void SetMaxHealth()
    {
        slider.maxValue = initHealth;
        slider.value = mCurrentHealth;
    }

    public void SetHealth()
    {
        slider.value = mCurrentHealth;
    }

    public EnemyBehavior GetEnemyBehavior() { return enemyBehavior; }

    public float GetSpeed() { return runSpeed; }

    public float GetAttackRate() { return attackRate; }

    public float GetAttackRange() { return attackRange; }

    public float GetTrackingDistance() { return trackingDistance; }

    public void Hurt(int damage, float force, Transform obj)
    {
        // show healthbar
        canvas.enabled = true;
        // animate hurt animation
        animator.SetTrigger("Hurt");
        // minus current health by damage
        mCurrentHealth -= damage;
        SetHealth();
        Knockback(obj, force);
        // if current drop below 0, play die animation and set enable to boc collider and scripts to
        // false
        if (mCurrentHealth <= 0)
        {
            Die();
        }
    }

    public void DamageOverTime(int damageAmount, int damageTime)
    {
        if (!isPoisoned)
            StartCoroutine(DamageOverTimeCoroutine(damageAmount, damageTime));
    }

    IEnumerator DamageOverTimeCoroutine(int damageAmount, int damageTime)
    {
        isPoisoned = true;
        while (damageTime > 0)
        {
            renderer.color = Color.green;
            mCurrentHealth -= damageAmount;
            SetHealth();
            if (mCurrentHealth < 1)
            {
                renderer.color = Color.white;
                Die();
            }
            yield return new WaitForSecondsRealtime(0.25f);
            renderer.color = Color.white;
            yield return new WaitForSecondsRealtime(0.75f);
            damageTime--;
        }
        isPoisoned = false;
    }

    public void Knockback(Transform obj, float knockbackForce)
    {
        if (obj.position.x - this.transform.position.x > 0)
        {
            rigidbody2D.AddForce(new Vector2(-2, 1) * knockbackForce, ForceMode2D.Impulse);
        }
        else
        {
            rigidbody2D.AddForce(new Vector2(2, 1) * knockbackForce, ForceMode2D.Impulse);
        }
    }

    public bool IsStun()
    {
        return isStun;
    }

    public IEnumerator Stun()
    {
        isStun = true;
        yield return new WaitForSecondsRealtime(stunDuration);
        isStun = false;
    }

    private void Die()
    {
        canvas.enabled = false;
        StopAllCoroutines();

        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        gameObject.GetComponent<EnemyNormalBehavior>().animator.SetBool("isDead", true);

        // Disable box collider, enemy normal behavior, this script
        GetComponent<Collider2D>().enabled = false;
        GetComponent<EnemyNormalBehavior>().enabled = false;
        this.enabled = false;
    }
}
