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
    [SerializeField] private float slowPercent = .25f;
    private int slowDuration = 5;
    [SerializeField] private float holyDamageModifier = 2f;

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
                FindObjectOfType<AudioManager>().Play("Swing");
                animator.SetTrigger("Attack"); // attack function is within the animation
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void AttackSound()
    {
        FindObjectOfType<AudioManager>().Play("Swing");
    }

    void Attack()
    {
        powerUp = weaponGlow.GetPowerUp();

        // fire powerup
        if (powerUp == 4)
            modifiedAttackDamage = baseAttackDamage + additionalFireDamage;
        else
            modifiedAttackDamage = baseAttackDamage;

        // run attack modifier
        if (animator.GetFloat("Speed") > 16)
            modifiedAttackDamage = modifiedAttackDamage + additionalRunDamage;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            // holy powerup
            if (powerUp == 2 && enemy.CompareTag("Undead"))
                modifiedAttackDamage = (int)(baseAttackDamage * holyDamageModifier);

            if (enemy.CompareTag("Boss"))
            {
                enemy.GetComponent<BossController>().Hurt(modifiedAttackDamage, attackForce, this.gameObject.transform);

                // poison powerup
                if (powerUp == 3)
                    enemy.GetComponent<BossController>().DamageOverTime(poisonDamage, poisonDuration);
                // ice powerup
                if (powerUp == 1)
                    enemy.GetComponent<BossController>().SlowOverTime(slowPercent, slowDuration);
            }
            // if enemy is not a ghost or powerUp is holy
            else if (enemy.GetComponent<EnemyController>().GetEnemyBehavior() != EnemyBehavior.GHOST || powerUp == 2)
            {
                enemy.GetComponent<EnemyController>().Hurt(modifiedAttackDamage, attackForce, this.gameObject.transform);

                // poison powerup
                if (powerUp == 3)
                    enemy.GetComponent<EnemyController>().DamageOverTime(poisonDamage, poisonDuration);
                // ice powerup
                if (powerUp == 1)
                    enemy.GetComponent<EnemyController>().SlowOverTime(slowPercent, slowDuration);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
