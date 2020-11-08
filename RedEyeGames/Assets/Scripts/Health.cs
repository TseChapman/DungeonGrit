using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    private int health;

    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        health = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (health < 1)
        {
            animator.SetBool("IsDead", true);
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            takeDamage(25);
            animator.SetBool("IsHurt", true);
        }
        else
        {
            animator.SetBool("IsHurt", false);
        }
    }

    void takeDamage(int damage)
    {
        health -= damage;
    }
}
