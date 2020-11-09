using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAttack : MonoBehaviour
{
    private Health health;

    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.4f;
    [SerializeField] private int attackDamage = 50;
    [SerializeField] private float attackRate = 2f;
    private float nextAttackTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health.IsDead())
            return;

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (!enemy.GetComponent<EnemyController>().IsDead())
                enemy.GetComponent<EnemyController>().Hurt(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
