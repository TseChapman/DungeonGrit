using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.IO;

public class EnemyNormalBehavior : MonoBehaviour
{
    public Seeker seeker;
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

    public EnemyBehavior mEnemyBehavior = EnemyBehavior.NUM_ENEMY_BEHAVIOR;
    private Vector2 mOffset;
    private float mAttackRange;
    private bool mIsRight = true;
    private float mAttackRate = 0f;
    private float mNextAttack;
    private float mTrackingRange = 0f;
    [SerializeField] private int mAttackDamage = 10;
    [SerializeField] private float mKnockbackForce = 5;
    [SerializeField] private float colliderTime = 0.2f;
    private int mCollisionDamage = 5;
    private bool isGrounded;


    private float mNextWayPointDist = 1f;
    private Pathfinding.Path mPath;
    private int mCurrentWayPoint = 0;
    private bool mReachedEndOfPath = false;

    // orc stuff
    [SerializeField] private int extraDamage = 0; // the 0 is for the initalizastion


    public Transform[] patrolPoints = null;
    private bool OrcRight = true;

    private int currentPatrolPoint = 0;


    // Start is called before the first frame update
    private void Start()
    {
        mEnemyBehavior = enemyController.GetEnemyBehavior();
        mOffset = boxCollider.offset; // .offset return a Vector2 (x,y) of the box collider
        mAttackRate = enemyController.GetAttackRate();
        mAttackRange = enemyController.GetAttackRange();
        mTrackingRange = enemyController.GetTrackingDistance();


        //patrolPoints = p

/*        patrolPoints = new Transform[patrolPoints.Length];
        
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            
            patrolPoints[i] = enemyController.patrolPoints[i];
        }*/


        currentPatrolPoint = 1;

        FindPath();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (enemyController.IsStun()) // cannot move stunned
            return;

        bool isGound = CheckIsGround();
        bool isBlock = CheckIsBlocked();
        
        
        if (mEnemyBehavior == EnemyBehavior.TRACKING || (mEnemyBehavior == EnemyBehavior.ORC && OrcTrackTrue()))
        {
            // Change direction to face hero if not already facing hero
            //UpdateSpeed(0f);

            TrackHero(isGound);
        }

        // Skeleton Patrol
        else if (mEnemyBehavior == EnemyBehavior.PATROL)
        {
            if (!isGound && IsGrounded() || isBlock)
            {
                RotateEnemy();
            }
            UpdateSpeed(enemyController.GetSpeed());
        }

        // temp comment

        /*// Orc Patrol
        else if(mEnemyBehavior == EnemyBehavior.ORC)
        {
            OrcRetreat();
        }*/

        else if (mEnemyBehavior == EnemyBehavior.GHOST)
        {
            UpdatePath();
        }
        else if (mEnemyBehavior == EnemyBehavior.FLY)
        {
            // FLY behavior logic should go here
        }
        CheckHeroDistance();
    }

    void OrcRetreat()
    {
        // update direction
        float dir = patrolPoints[currentPatrolPoint].position.x - transform.position.x;
        
        // if we are already there
        if (/*Vector2.Distance(patrolPoints[currentPatrolPoint].position, transform.position)*/ Mathf.Abs(dir) < 1f)
        {
            OrcRight = !OrcRight;
            /*currentPatrolPoint++;
            if (currentPatrolPoint > patrolPoints.Length)
            {
                currentPatrolPoint = 0;
            }*/


            // change orc direction point
            if(OrcRight)
            {
                currentPatrolPoint = 1;
            }
            else
            {
                currentPatrolPoint = 0;
            }

            // update direction
            dir = patrolPoints[currentPatrolPoint].position.x - transform.position.x;
        }

        //print(dir);
        
        // Face direction of reteat
        if (dir > 0) // right
        {
            transform.eulerAngles = new Vector2(0, 0);
        }

        else // left
        {
            transform.eulerAngles = new Vector2(0, 180);
        }

        /*RotateEnemy();*/

        // Start moving 
        UpdateSpeed(enemyController.GetSpeed());
    }



    bool OrcTrackTrue()
    {
        bool orcTrack;
                        // Distance < tracking range
        if (Vector3.Distance(attackPoint.transform.position, hero.transform.position) < mTrackingRange)
        {
            orcTrack = true;
        }

        else
        {
            orcTrack = false;
        }

        return orcTrack;
    }

    private void FindPath()
    {
        //Debug.Log("FINDING PATH");
        if (seeker == null || enemyController.GetIsDead() is true)
            return;

        InvokeRepeating("FindPathTrack", 0f, .5f);
    }

    private void FindPathTrack()
    {
        Debug.Log("Find Path");
        if (seeker.IsDone() || enemyController.GetIsDead() is false)
            seeker.StartPath(rb.position, hero.transform.position, OnPathComplete);
    }

    private void OnPathComplete(Pathfinding.Path p)
    {
        if (!p.error)
        {
            mPath = p;
            mCurrentWayPoint = 0;
        }
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
        if (transform.name == "Ghost")
        {
            //Debug.Log(speed);
        }
        animator.SetFloat("Speed", speed);
        rb.velocity = velocity;
    }

    // Attack hero based on attack cooldown
    private void Attack()
    {
        Collider2D hitHero =
            Physics2D.OverlapCircle(attackPoint.transform.position, mAttackRange, heroLayer);

        if (hitHero)
        {
            FindObjectOfType<AudioManager>().Play("Swing");
            hero.GetComponent<Health>().TakeDamage(mAttackDamage, mKnockbackForce, this.transform);
        }
            /*
            if(this.GetComponent<Health>().GetHealth() < this.GetComponent<Health>().GetMaxHealth() * (3 / 4) && mEnemyBehavior == EnemyBehavior.ORC) // if has less then 3/4 health (and an orc) take extra damage
                hero.GetComponent<Health>().TakeDamage(mAttackDamage + extraDamage, mKnockbackForce, this.transform);
            else
            */
            
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
            if (Time.time >= mNextAttack && !hero.GetComponent<HeroMovement>().IsRolling())
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

            /*else if (this.GetComponent<Health>().GetHealth() < this.GetComponent<Health>().GetMaxHealth() * (1 / 4) && mEnemyBehavior == EnemyBehavior.ORC)
                UpdateSpeed(enemyController.GetSpeed() * 3/2); // if the orc health is less then 25% then increase speed to 1.5 original*/

            else
                UpdateSpeed(enemyController.GetSpeed());
        }
        else
        {
            UpdateSpeed(0f);
        }
    }

    private void UpdatePath()
    {
        if (mPath == null)
            return;

        if (mCurrentWayPoint >= mPath.vectorPath.Count)
        {
            mReachedEndOfPath = true;
            return;
        }
        else
        {
            mReachedEndOfPath = false;
        }
        RotateTowardHero();
        Vector2 direction = ((Vector2)mPath.vectorPath[mCurrentWayPoint] - rb.position).normalized;
        Vector2 force = direction * enemyController.GetSpeed() * Time.deltaTime;
        rb.AddForce(force);
        //Debug.Log(force.x);
        float dist = Vector2.Distance(rb.position, mPath.vectorPath[mCurrentWayPoint]);

        if (dist < mNextWayPointDist)
        {
            mCurrentWayPoint++;
        }
    }

    public int CollisionDamage() { return mCollisionDamage; }

    public float KnockbackForce() { return mKnockbackForce; }

    private Vector2 ToVector2(Vector3 vec3)
    {
        return new Vector2(vec3.x, vec3.y);
    }

    public void setHero(GameObject theHero)
    {
        hero = theHero;
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
