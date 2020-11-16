using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Slider slider;

    private bool isDead;
    [SerializeField] private bool isGodMode = false;

    [SerializeField] private HeroMovement heroMovement;
    [SerializeField] private Animator animator;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int health;

    // Start is called before the first frame update
    void Start()
    {
        heroMovement = GetComponent<HeroMovement>();
        maxHealth = 100;
        health = maxHealth;
        SetMaxHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            TakeDamage(25, 5f, this.transform);
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            GainHealth(50);
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetMaxHealth()
    {
        slider.maxValue = maxHealth;
        slider.value = health;
    }

    public float GetHealth()
    {
        return health;
    }

    public void SetHealth()
    {
        slider.value = health;
    }

    public void GainHealth(int health)
    {
        this.health += health;

        if (this.health > maxHealth)
            this.health = maxHealth;

        SetHealth();
    }

    public void TakeDamage(int damage, float knockbackForce, Transform obj)
    {
        if (isDead)
            return;

        heroMovement.Knockback(obj, knockbackForce);
        animator.SetBool("IsJumping", false);

        if (isGodMode)
            return;

        health -= damage;
        SetHealth();

        animator.SetTrigger("Hurt");

        if (health < 1)
            Death();
    }

    private void Death()
    {
        isDead = true;
        animator.SetBool("IsDead", true);
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void setGodMode()
    {
        isGodMode = true;
    }
}
