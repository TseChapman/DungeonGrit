using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyBehavior { PATROL = 0, FLY = 1, TRACKING = 2, GUARD = 3, GHOST = 4, NUM_ENEMY_BEHAVIOR = 5 }

public class EnemyController : MonoBehaviour
{
    public Slider slider;

    public EnemyBehavior enemyBehavior;
    private Rigidbody2D mRigidbody2D;
    private Canvas canvas;
    private SpriteRenderer mRenderer;
    private Animator animator;
    private ItemManager mItemManager;

    [SerializeField] private int initHealth = 0;
    [SerializeField] private int mCurrentHealth;
    [SerializeField] private float runSpeed = 0f;
    [SerializeField] private float attackRate = 0f;
    [SerializeField] private float attackRange = 0f;
    [SerializeField] private float trackingDistance = 0f; // Only used when enemyBehavior is Tracking

    [SerializeField] private float knockbackDuration = 0.5f; // should match hurt and stun animation
    [SerializeField] private float forceX = 2;
    [SerializeField] private float forceY = 1;
    [SerializeField] private bool isStun = false;
    [SerializeField] private float stunDuration = 0.5f;
    private bool isPoisoned = false;

    // Start is called before the first frame update
    private void Start()
    {
        mItemManager = GameObject.FindObjectOfType<ItemManager>();
        mCurrentHealth = initHealth;
        mRigidbody2D = GetComponent<Rigidbody2D>();
        canvas = GetComponentInChildren<Canvas>();
        mRenderer = GetComponent<SpriteRenderer>();
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

    public bool GetIsDead() { return (mCurrentHealth <= 0f); }

    public EnemyBehavior GetEnemyBehavior() { return enemyBehavior; }

    public float GetSpeed() { return runSpeed; }

    public float GetAttackRate() { return attackRate; }

    public float GetAttackRange() { return attackRange; }

    public float GetTrackingDistance() { return trackingDistance; }

    public void Hurt(int damage, float force, Transform obj)
    {
        // show healthbar
        canvas.enabled = true;
        // minus current health by damage
        mCurrentHealth -= damage;
        SetHealth();
        // animate hurt animation
        animator.SetTrigger("Hurt");
        StartCoroutine(Knockback(knockbackDuration, obj, force));
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
            mRenderer.color = Color.green;
            mCurrentHealth -= damageAmount;
            SetHealth();
            if (mCurrentHealth < 1)
            {
                mRenderer.color = Color.white;
                Die();
            }
            yield return new WaitForSecondsRealtime(0.25f);
            mRenderer.color = Color.white;
            yield return new WaitForSecondsRealtime(0.75f);
            damageTime--;
        }
        isPoisoned = false;
    }

    IEnumerator Knockback(float knockbackDuration, Transform obj, float knockbackForce)
    {
        float timer = 0;

        while (knockbackDuration > timer)
        {
            timer += Time.deltaTime;
            Vector2 direction = (obj.position - this.transform.position).normalized;
            if (obj.position.x - this.transform.position.x > 0)
            {
                mRigidbody2D.AddForce(new Vector2(-forceX, forceY) * knockbackForce);
            }
            else
            {
                mRigidbody2D.AddForce(new Vector2(forceX, forceY) * knockbackForce);
            }
        }

        yield return 0;
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

        // Spawn Item
        mItemManager.DropRandom(gameObject.transform);

        // Disable box collider, enemy normal behavior, this script
        GetComponent<Collider2D>().enabled = false;
        GetComponent<EnemyNormalBehavior>().enabled = false;
        this.enabled = false;
    }
}
