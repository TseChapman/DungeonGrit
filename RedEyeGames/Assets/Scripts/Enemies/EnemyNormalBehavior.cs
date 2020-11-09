using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNormalBehavior : MonoBehaviour
{
    public EnemyController enemyController;
    public Animator animator;
    public LayerMask plateformMask;
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider;
    public GameObject enemyPosition;
    public GameObject groundCheck;
    public GameObject attackPoint;
    public GameObject hero;
    public LayerMask heroLayer;

    private EnemyBehavior mEnemyBehavior = EnemyBehavior.NUM_ENEMY_BEHAVIOR;
    private Vector2 mOffset;
    private float mAttackRange;
    private bool mIsRight = true;
    private float mAttackRate = 0f;
    private float mNextAttack;
    private float mTrackingRange = 0f;
    private int mAttackDamage = 10;

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
        if (enemyController.IsDead())
        {
            rb.velocity = new Vector2(0, 0);
            this.enabled = false;
            return;
        }

        bool isGound = CheckIsGround();
        if (mEnemyBehavior == EnemyBehavior.PATROL)
        {
            if (!isGound)
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

    // Check if it reaches the edge
    private bool CheckIsGround()
    {
        Vector2 lineCastPos = new Vector2(groundCheck.transform.position.x, groundCheck.transform.position.y);
        Debug.DrawLine(lineCastPos, lineCastPos + Vector2.down);
        Vector2 blockLineCastPos = new Vector2(groundCheck.transform.position.x, groundCheck.transform.position.y + boxCollider.size.y / 2f);
        Vector3 right = -gameObject.transform.right;
        Debug.DrawLine(blockLineCastPos, blockLineCastPos - ToVector2(right) * 0.2f);
        bool isGound = Physics2D.Linecast(lineCastPos, lineCastPos + Vector2.down, plateformMask);
        bool isBlocked = Physics2D.Linecast(blockLineCastPos, blockLineCastPos - ToVector2(right) * 0.2f, plateformMask);
        // Debug.Log(isGound);

        return isGound || isBlocked;
    }

    // Change speed to move
    private void UpdateSpeed(float speed)
    {
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

        if (hitHero && Time.time >= mNextAttack)
        {
            hero.GetComponent<Health>().TakeDamage(mAttackDamage);

            //Debug.Log("Hit hero");
            animator.SetBool("isAttack", true);
            mNextAttack = Time.time + 1f / mAttackRate;
        }
    }

    // Check if hero is within attack range, if so, attack
    private void CheckHeroDistance()
    {
        float dist = Vector3.Distance(attackPoint.transform.position, hero.transform.position);
        if (dist <= mAttackRange)
        {
            RotateTowardHero();
            UpdateSpeed(0f);
            Attack();
        }
        else
        {
            animator.SetBool("isAttack", false);
        }
    }

    // Used in Tracking Behavior only: rotate toward hero
    private void RotateTowardHero()
    {
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

    private Vector2 ToVector2(Vector3 vec3)
    {
        return new Vector2(vec3.x, vec3.y);
    }
}
