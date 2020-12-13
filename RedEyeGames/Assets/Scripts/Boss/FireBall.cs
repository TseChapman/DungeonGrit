using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float knockbackForce = 5f;
    private Vector3 direction = Vector3.zero;
    private bool isExplode = false;

    private float mTimer;

    public void SetDamage(int _damage) { damage = _damage; }

    public void SetDirection(Vector3 directionNormalized)
    {
        direction = directionNormalized;
        transform.right = directionNormalized;
    }
    private void Start()
    {
        mTimer = 0;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (direction == Vector3.zero)
            return;
        mTimer += Time.smoothDeltaTime;
        if (isExplode is false)
            transform.position += direction * speed * Time.smoothDeltaTime;
        if (mTimer > 10f)
        {
            animator.SetBool("Explosion", true);
            isExplode = true;
        }
    }

    private void DestroyObject()
    {
        Destroy(transform.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            collision.gameObject.GetComponent<Health>().TakeDamage(damage, knockbackForce, this.transform);
        animator.SetBool("Explosion", true);
        //Debug.Log("Explosion");
        isExplode = true;
    }
}
