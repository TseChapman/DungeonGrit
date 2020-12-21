using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrap : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private int trapDamage = 10;
    [SerializeField] private float trapKnockbackForce = 5f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        animator.SetTrigger("Activate");
    }

    public void ActivateBearTrap()
    {
        Collider2D[] collisionArray = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (Collider2D collision in collisionArray)
        {
            if (collision.CompareTag("Player"))
                collision.GetComponent<Health>().TakeDamage(trapDamage, trapKnockbackForce, this.transform);
            if (collision.CompareTag("Enemy") || collision.CompareTag("Undead"))
                collision.GetComponent<EnemyController>().Hurt(trapDamage, 1f, this.transform);
            GetComponent<BoxCollider2D>().enabled = false;
            this.enabled = false;
        }
    }
}
