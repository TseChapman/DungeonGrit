using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyBehavior { PATROL = 0, FLY = 1, TRACKING = 2, GUARD = 3, GHOST = 4, ORC = 5, BOSS = 6, NUM_ENEMY_BEHAVIOR = 7 }

public class EnemyController : MonoBehaviour
{
    public Slider slider;

    public EnemyBehavior enemyBehavior;
    private Rigidbody2D mRigidbody2D;
    private Canvas canvas;
    private SpriteRenderer mRenderer;
    private Animator animator;
    private ItemManager mItemManager;

    [SerializeField] public int initHealth = 0;
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
    [SerializeField] private static int iceDebuff = 0;
    [SerializeField] private float gVal = 180f;
    [SerializeField] private int numIceHits = 0;
    [SerializeField] private bool frozen = false;
    [SerializeField] private int iceImmuneTime = 5;

    [SerializeField] public Transform[] patrolPoints;

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

        DamagePopUp.CreateEnemy(transform.position, damage);

        // animate hurt animation
        FindObjectOfType<AudioManager>().Play("Hurt");
        animator.SetTrigger("Hurt");
        StartCoroutine(Knockback(knockbackDuration, obj, force));
        // if current drop below 0, play die animation and set enable to boc collider and scripts to
        // false
        if (mCurrentHealth <= 0)
        {
            Die();
        }
    }

    public void SlowOverTime(float slowPercent, int slowTime)
    {
        if (!frozen)
        {
            if (numIceHits == 0)
            {
                // increment numIceHits
                numIceHits++;
                // start debuff coroutine
                StartCoroutine(SlowOverTimeCoroutine(slowPercent, slowTime));
            }
            else if (numIceHits < 4)
            {
                // increment numIceHits
                numIceHits++;
                // update the slow debuff timer
                iceDebuff = slowTime;
                // reduce enemy move speed
                runSpeed -= (runSpeed * slowPercent);
                // make blue tint darker blue
                gVal -= 20;
                // apply blue tint to sprite
                mRenderer.color = new Color(0f / 255f, gVal / 255f, 255f / 255f);
            }
            else if (numIceHits == 4)
            {
                // update the slow debuff timer
                iceDebuff = slowTime;
                // set frozen to true
                frozen = true;
                // freeze enemy in place
                runSpeed = 0;
                // make tint dark blue
                mRenderer.color = new Color(0f / 255f, 0f / 255f, 255f / 255f);
            }
        }
        
            
    }

    IEnumerator SlowOverTimeCoroutine(float slowPercent, int slowTime)
    {
        // save the enemy's speed so we can restore it once the slow debuff
        // wears off
        float preSlowSpeed = runSpeed;
        // save debuff time to instance variable
        iceDebuff = slowTime;
        // reduce enemy move speed
        runSpeed -= (runSpeed * slowPercent);
        // apply blue tint to sprite
        mRenderer.color = new Color(0f / 255f, gVal / 255f, 255f / 255f);
        while (iceDebuff > 0)
        {
            iceDebuff--;
            yield return new WaitForSecondsRealtime(1f);
        }
        // return enemy to original color
        mRenderer.color = Color.white;
        // return enemy to original speed
        runSpeed = preSlowSpeed;
        // reset numIceHits counter
        numIceHits = 0;
        // if the enemy was actually frozen
        if (frozen)
            // start frozen coroutine
            StartCoroutine(FrozenCoroutine(iceImmuneTime));
    }

    IEnumerator FrozenCoroutine(int immuneTime)
    {
        while (immuneTime > 0)
        {
            immuneTime--;
            yield return new WaitForSecondsRealtime(1f);
        }
        frozen = false;
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
            DamagePopUp.CreateEnemy(transform.position, damageAmount);
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
        // disable health bar
        canvas.enabled = false;

        // disable all coroutines
        StopAllCoroutines();

        // set death animation
        FindObjectOfType<AudioManager>().Play("Death");
        gameObject.GetComponent<EnemyNormalBehavior>().animator.SetBool("isDead", true);

        // Spawn Item
        mItemManager.DropRandom(gameObject.transform);

        // ragdoll effect
        StartCoroutine(BodyFall());

        // Disable box collider, enemy normal behavior, this script
        GetComponent<EnemyNormalBehavior>().enabled = false;
        this.enabled = false;
    }

    IEnumerator BodyFall()
    {
        while (!GetComponent<EnemyNormalBehavior>().IsGrounded())
            yield return new WaitForFixedUpdate();
        GetComponent<Collider2D>().enabled = false;
        mRigidbody2D.bodyType = RigidbodyType2D.Static;
        StopAllCoroutines();
        GetComponent<EnemyNormalBehavior>().enabled = false;
        this.enabled = false;
        yield return 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("OutBounds"))
        {
            mRigidbody2D.bodyType = RigidbodyType2D.Static;
            Die();
        }
    }
}
