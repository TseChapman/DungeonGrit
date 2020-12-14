using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    public Animator animator;
    // Use to Detect Layers
    public LayerMask heroLayer;
    public LayerMask plateformMask;
    public LayerMask trapMask;
    public LayerMask enemyMask;

    // use to Detect position
    public GameObject enemyPosition;
    public GameObject groundCheck;
    public GameObject attackPoint;
    public GameObject longRangeAttackPos;
    public GameObject hero; // Use to track the target
    public GameObject fireBallPrefab;

    // Boss Component
    private BossController bossController;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    // Parameters
    [SerializeField] private int mAttackDamage = 10;
    [SerializeField] private float mKnockbackForce = 5;
    private EnemyBehavior mEnemyBehavior;
    private Vector2 mOffset;
    private float mAttackRange;
    private float mAttackRate;
    private float mLongRangeAttackRate;
    private float mNextAttack;
    private float mNextLongRangeAttack;
    private int mCollisionDamage;
    private bool mIsRight;
    private bool isGrounded;
    private bool mIsAwake;
    private bool mIsAlreadyAwake;
    private int mNumAttackToLightning;
    private bool mIsAttack;

    private float colliderTime = 0.2f;

    public void SetAttackDamage(float attackMultiplier) { mAttackDamage = (int)((float)mAttackDamage * attackMultiplier); }

    public int CollisionDamage() { return mCollisionDamage; }

    public float KnockbackForce() { return mKnockbackForce; }

    public bool IsGrounded()
    {
        isGrounded = false;

        Collider2D[] colliders =
            Physics2D.OverlapCircleAll(groundCheck.transform.position, 0.05f, plateformMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                isGrounded = true;
            }
        }
        return isGrounded;
    }

    // Start is called before the first frame update
    private void Start()
    {
        InitComponents();
        InitParameters();
    }

    private void InitComponents()
    {
        bossController = GetComponent<BossController>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void InitParameters()
    {
        mEnemyBehavior = bossController.GetEnemyBehavior();
        mOffset = boxCollider.offset;
        mAttackRange = bossController.GetAttackRange();
        mAttackRate = bossController.GetAttackRate();
        mLongRangeAttackRate = bossController.GetLongRangeAttackRate();
        mNextAttack = Time.time;
        mNextLongRangeAttack = Time.time;
        mCollisionDamage = 5;
        mIsRight = true;
        mIsAwake = false;
        mIsAlreadyAwake = false;
        mNumAttackToLightning = 3;
        mIsAttack = false;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (mEnemyBehavior != EnemyBehavior.BOSS) // valid check
            return;

        CheckAwake();
        if (mIsAwake is false) // If the boss is not awaken
            return;

        if (bossController.IsStun()) // cannot move stunned
            return;

        if (mIsAlreadyAwake is false && mIsAwake is true)
        {
            AwakeBoss();
        }


        // Main update calls:
        if (mIsAlreadyAwake is true)
        {
            bool isGround = CheckIsGround();
            TrackHero(isGround);
            Attack();
        }
    }

    private void CheckAwake()
    {
        if (mIsAlreadyAwake is true)
            return;
        mIsAwake =  Vector2.Distance(enemyPosition.transform.position, hero.transform.position) <= bossController.GetAwakeDistance();
        //Debug.Log(mIsAwake);
    }

    private void AwakeBoss()
    {
        // Play awake animation
        animator.SetBool("isAwake", true);
        mIsAlreadyAwake = true;
    }

    // Check if it reaches the edge
    private bool CheckIsGround()
    {
        Vector2 lineCastPos = new Vector2(groundCheck.transform.position.x, groundCheck.transform.position.y);
        Debug.DrawLine(lineCastPos, lineCastPos + Vector2.down);
        bool isGound = Physics2D.Linecast(lineCastPos, lineCastPos + Vector2.down, plateformMask);

        return isGound;
    }

    private void SetIsAttackTrue()
    {
        mIsAttack = true;
    }

    private void SetIsAttackFalse()
    {
        mIsAttack = false;
    }

    // Attack hero based on attack cooldown
    private void MeleeAttack()
    {
        Collider2D hitHero =
            Physics2D.OverlapCircle(attackPoint.transform.position, mAttackRange, heroLayer);

        if (hitHero)
            hero.GetComponent<Health>().TakeDamage(mAttackDamage, mKnockbackForce, this.transform);
    }

    private void LongRangeAttack()
    {
        // Instantiate a fire ball
        GameObject fireball = Instantiate(fireBallPrefab, longRangeAttackPos.transform.position, Quaternion.identity);
        // set it toward hero
        Vector3 direction = (hero.transform.position - fireball.transform.position).normalized;
        fireball.GetComponent<FireBall>().SetDamage(mAttackDamage);
        fireball.GetComponent<FireBall>().SetDirection(direction);
    }

    // Rotate Enemy 180 degree and position enemy based on offset
    private void RotateEnemy()
    {
        Vector2 position = transform.position;
        mIsRight = (mIsRight) ? false : true;
        if (mIsRight)
        {
            // Debug.Log("Turn to right");
            position.x = position.x + 2 * -mOffset.x;
        }
        else
        {
            // Debug.Log("Turn to left");
            position.x = position.x + 2 * mOffset.x;
        }
        transform.position = position;
        Vector3 rotation = transform.eulerAngles;
        rotation.y += 180;
        transform.eulerAngles = rotation;
    }

    // Used in Tracking Behavior only: rotate toward hero
    private void RotateTowardHero()
    {
        if (bossController.IsStun()) // cannot move if stunned
            return;

        if (enemyPosition.transform.position.x > hero.transform.position.x && mIsRight is true)
        {
            RotateEnemy();
        }
        else if (enemyPosition.transform.position.x < hero.transform.position.x &&
                 mIsRight is false)
        {
            RotateEnemy();
        }
    }

    // Change speed to move
    public void UpdateSpeed(float speed)
    {
        if (bossController.IsStun()) // cannot move stunned
            return;

        Vector2 velocity = rb.velocity;
        velocity.x = transform.right.x * speed * Time.smoothDeltaTime;
        animator.SetFloat("Speed", speed);
        rb.velocity = velocity;
        //Debug.Log(velocity);
    }

    private void TrackHero(bool isGround)
    {
        if (bossController.IsStun()) // cannot move if stunned
            return;

        RotateTowardHero();
        if (!isGround || mIsAttack is true)
            UpdateSpeed(0f);
        else if (mIsAttack is false)
            UpdateSpeed(bossController.GetSpeed());
    }

    private void Attack()
    {
        if (bossController.IsStun()) // cannot attack if stunned
            return;

        float dist = Vector2.Distance(transform.position, hero.transform.position);
        if (Time.time >= mNextAttack && dist <= mAttackRange)
        {
            //UpdateSpeed(0f);
            FindObjectOfType<AudioManager>().Play("Swing");
            animator.SetTrigger("Attack");
            mNextAttack = Time.time + 1f / mAttackRate; // Next attack time
            mNumAttackToLightning++;
        }
        else if (Time.time >= mNextLongRangeAttack && dist >= mAttackRange)
        {
            //UpdateSpeed(0f);
            FindObjectOfType<AudioManager>().Play("FireBallShot");
            animator.SetTrigger("LongRangeAttack");
            mNextLongRangeAttack = Time.time + 1f / mLongRangeAttackRate;
            mNumAttackToLightning++;
        }
    }

    public void ActivateCollider()
    {
        StartCoroutine(ActivateColliderCoroutine());
    }

    IEnumerator ActivateColliderCoroutine()
    {
        yield return new WaitForSecondsRealtime(colliderTime);
        boxCollider.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;

        yield return 0;
    }
}
