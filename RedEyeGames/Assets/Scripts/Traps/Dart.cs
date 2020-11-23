using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dart : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float knockbackForce = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += ((speed * Time.smoothDeltaTime) * transform.right);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            collision.gameObject.GetComponent<Health>().TakeDamage(damage, knockbackForce, this.transform);
        if (collision.collider.CompareTag("Enemy"))
            collision.gameObject.GetComponent<EnemyController>().Hurt(damage, knockbackForce, this.transform);

        Destroy(gameObject);
    }
}
