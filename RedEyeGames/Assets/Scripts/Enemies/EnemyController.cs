using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyBehavior { PATROL = 0, FLY = 1, TRACKING = 2, GUARD = 3, NUM_ENEMY_BEHAVIOR = 4 }

public class EnemyController : MonoBehaviour
{
    public EnemyBehavior enemyBehavior;

    [SerializeField]
    private int initHealth = 0;
    [SerializeField]
    private float runSpeed = 0f;
    [SerializeField]
    private float attackRate = 0f;
    [SerializeField]
    private float attackRange = 0f;

    [SerializeField]
    private float trackingDistance = 0f; // Only used when enemyBehavior is Tracking

    private int mCurrentHealth;

    public EnemyBehavior GetEnemyBehavior() { return enemyBehavior; }

    public float GetSpeed() { return runSpeed; }

    public float GetAttackRate() { return attackRate; }

    public float GetAttackRange() { return attackRange; }

    public float GetTrackingDistance() { return trackingDistance; }

    public void Hurt(int damage)
    {
        // minus current health by damage
        mCurrentHealth -= damage;
        // animate hurt animation
        gameObject.GetComponent<EnemyNormalBehavior>().animator.SetTrigger("Hurt");
        // if current drop below 0, play die animation and set enable to boc collider and scripts to
        // false
        if (mCurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        gameObject.GetComponent<EnemyNormalBehavior>().animator.SetBool("isDead", true);

        // Disable box collider, enemy normal behavior, this script
    }

    // Start is called before the first frame update
    private void Start() { mCurrentHealth = initHealth; }
}
