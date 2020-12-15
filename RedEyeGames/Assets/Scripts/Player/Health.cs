using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Slider slider;

    private bool isDead;
    private bool isGod = false;
    [SerializeField] private bool halfDamage = false;

    [SerializeField] private HeroMovement heroMovement;
    [SerializeField] private Animator animator;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int health;

    public void SetIsGod(bool isGodActive)
    {
        isGod = isGodActive;
    }

    public void HalfDamage(bool set)
    {
        halfDamage = set;
    }

    public void Death()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        isDead = true;
        health = 0;
        SetHealth();

        FindObjectOfType<AudioManager>().Play("Death");

        animator.SetBool("IsDead", true);

        FindObjectOfType<GameOverMenu>().GameOver();
    }

    public void DisableRenderer()
    {
        GetComponent<Renderer>().enabled = false;
    }

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

        /*if (Input.GetKeyDown(KeyCode.KeypadMinus))
            TakeDamage(25, 5f, this.transform);
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            GainHealth(50);*/
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
        
        if (isGod is false)
        {
            if (halfDamage)
                damage = damage / 2;
            health -= damage;
            SetHealth();
            DamagePopUp.CreatePlayer(transform.position, damage);
        }

        heroMovement.Knockback(obj, knockbackForce);
        animator.SetBool("IsJumping", false);

        FindObjectOfType<AudioManager>().Play("Hurt");

        animator.SetTrigger("Hurt");

        if (health < 1)
            Death();
    }

    public bool IsDead()
    {
        return isDead;
    }
}
