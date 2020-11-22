using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNormalBehavior : MonoBehaviour
{
    public LayerMask heroLayer;
    public LayerMask plateformMask;
    public LayerMask trapMask;
    public LayerMask enemyMask;
    public EnemyController enemyController;
    public Animator animator;
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider;
    public GameObject enemyPosition;
    public GameObject groundCheck;
    public GameObject attackPoint;
    public GameObject hero;

    private EnemyBehavior mEnemyBehavior = EnemyBehavior.NUM_ENEMY_BEHAVIOR;
    private Vector2 mOffset;
    private float mAttackRange;
    private bool mIsRight = true;
    private float mAttackRate = 0f;
    private float mNextAttack;
    private float mTrackingRange = 0f;
    [SerializeField] private int mAttackDamage = 10;
    private float mKnockbackForce = 5;
    private int mCollisionDamage = 5;
    private bool isGrounded;

    // Start is called before the first frame update
    private void Start()
    {
        mEnemyBehavior = enemyController.GetEnemyBehavior();
        mOffset = boxCollider.offset; // .offset return a Vector2 (x,y) of the box collider
        mAttackRate = enemyController.GetAttackRate();
        mAttackRange = enemyController.GetAttackRange();
        mTrackingRange = enemyController.GetTrackingDistance();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (enemyController.IsStun()) // cannot move stunned
            return;

        bool isGound = CheckIsGround();
        bool isBlock = CheckIsBlocked();
        if (mEnemyBehavior == EnemyBehavior.PATROL)
        {
            if (!isGound && IsGrounded() || isBlock)
            {
                RotateEnemy();
            }
            UpdateSpeed(enemyController.GetSpeed());
        }
        else if (mEnemyBehavior == EnemyBehavior.TRACKING)
        {
            TrackHero(isGound);
        }
        CheckHeroDistance();
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

    // Check if it reaches the edge
    private bool CheckIsGround()
    {
        Vector2 lineCastPos = new Vector2(groundCheck.transform.position.x, groundCheck.transform.position.y);
        Debug.DrawLine(lineCastPos, lineCastPos + Vector2.down);
        bool isGound = Physics2D.Linecast(lineCastPos, lineCastPos + Vector2.down, plateformMask);

        return isGound;
    }

    private bool CheckIsBlocked()
    {
        Vector2 blockLineCastPos = new Vector2(groundCheck.transform.position.x, groundCheck.transform.position.y + boxCollider.size.y / 2f);
        Vector3 right = -gameObject.transform.right;
        Debug.DrawLine(blockLineCastPos, blockLineCastPos - ToVector2(right) * 0.2f);
        bool isBlockByPlateform = Physics2D.Linecast(blockLineCastPos, blockLineCastPos - ToVector2(right) * 0.2f, plateformMask);
        bool isBlockByEnemy = Physics2D.Linecast(blockLineCastPos, blockLineCastPos - ToVector2(right) * 0.2f, enemyMask);
        bool isBlockByTrap = Physics2D.Linecast(blockLineCastPos, blockLineCastPos - ToVector2(right) * 0.2f, trapMask);
        return isBlockByPlateform || isBlockByEnemy || isBlockByTrap;
    }

    // Change speed to move
    public void UpdateSpeed(float speed)
    {
        if (enemyController.IsStun()) // cannot move stunned
            return;

        Vector2 velocity = rb.velocity;
        velocity.x = transform.right.x * speed * Time.smoothDeltaTime;
        animator.SetFloat("Speed", speed);
        rb.velocity = velocity;
    }

    // Attack hero based on attack cooldown
    private void Attack()
    {
        Collider2D hitHero =
            Physics2D.OverlapCircle(attackPoint.transform.position, mAttackRange, heroLayer);

        if (hitHero)
            hero.GetComponent<Health>().TakeDamage(mAttackDamage, mKnockbackForce, this.transform);
    }

    // Check if hero is within attack range, if so, attack
    private void CheckHeroDistance()
    {
        if (enemyController.IsStun()) // cannot attack if stunned
            return;

        float dist = Vector3.Distance(attackPoint.transform.position, hero.transform.position);
        if (dist <= mAttackRange)
        {
            RotateTowardHero();
            UpdateSpeed(0f);
            if (Time.time >= mNextAttack)
            {
                animator.SetTrigger("Attack"); // attack function is called within the animator
                mNextAttack = Time.time + 1f / mAttackRate;
            }
        }
    }

    // Used in Tracking Behavior only: rotate toward hero
    private void RotateTowardHero()
    {
        if (enemyController.IsStun()) // cannot move if stunned
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

    // Tracking Behavior: Track toward hero within tracking range
    private void TrackHero(bool isGound)
    {
        if (enemyController.IsStun()) // cannot move if stunned
            return;

        float dist = Vector3.Distance(enemyPosition.transform.position, hero.transform.position);
        if (dist <= mTrackingRange)
        {
            RotateTowardHero();
            if (!isGound)
            {
                UpdateSpeed(0f);
            }
            else
                UpdateSpeed(enemyController.GetSpeed());
        }
        else
        {
            UpdateSpeed(0f);
        }
    }

    public int CollisionDamage() { return mCollisionDamage; }

    public float KnockbackForce() { return mKnockbackForce; }

    private Vector2 ToVector2(Vector3 vec3)
    {
        return new Vector2(vec3.x, vec3.y);
    }
}
