using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAttack : MonoBehaviour
{
    private Health health;

    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private WeaponGlow weaponGlow;
    [SerializeField] private int powerUp;
    [SerializeField] private float attackRange = 0.4f;
    [SerializeField] private int baseAttackDamage = 10;
    private int modifiedAttackDamage;
    [SerializeField] private float attackRate = 2f;
    [SerializeField] private float attackForce = 1f;
    private float nextAttackTime = 0f;
    [SerializeField] private int additionalFireDamage = 5;
    [SerializeField] private int additionalRunDamage = 5;
    [SerializeField] private int poisonDamage = 1;
    [SerializeField] private int poisonDuration = 5;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        weaponGlow = GetComponent<WeaponGlow>();
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
                animator.SetTrigger("Attack"); // attack function is within the animation
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        powerUp = weaponGlow.getPowerUp();

        if (powerUp == 4)
            modifiedAttackDamage = baseAttackDamage + additionalFireDamage;
        else
            modifiedAttackDamage = baseAttackDamage;

        if (animator.GetFloat("Speed") > 16)
            modifiedAttackDamage = modifiedAttackDamage + additionalRunDamage;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyController>().Hurt(modifiedAttackDamage, attackForce, this.transform);
            if (powerUp == 3)
                enemy.GetComponent<EnemyController>().DamageOverTime(poisonDamage, poisonDuration);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
