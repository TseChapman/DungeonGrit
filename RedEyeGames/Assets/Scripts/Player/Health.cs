using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Slider slider;

    private bool isDead;

    [SerializeField] private Animator animator;
    [SerializeField] private int maxHealth = 100;
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

        if (Input.GetKeyDown(KeyCode.Backspace))
            TakeDamage(25);
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

    public void GainHealth(int health)
    {
        this.health += health;

        SetHealth();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;
        
        health -= damage;
        animator.SetTrigger("Hurt");

        SetHealth();

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
}
