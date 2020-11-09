using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Slider slider;

    private int maxHealth;
    private bool isDead;

    [SerializeField] private Animator animator;
    [SerializeField] private int health;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 100;
        health = maxHealth;
        SetMaxHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;

        SetHealth();

        if (health < 1)
        {
            Death();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            TakeDamage(25);
            animator.SetBool("IsHurt", true);
        }
        else
        {
            animator.SetBool("IsHurt", false);
        }
    }

    public void SetMaxHealth()
    {
        slider.maxValue = maxHealth;
        slider.value = health;
    }

    public void SetHealth()
    {
        slider.value = health;
    }

    private void GainHealth(int health)
    {
        this.health += health;
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
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

}
