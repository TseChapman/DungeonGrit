using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMovement : MonoBehaviour
{
    private Rigidbody2D rigidBody2D;
    private PolygonCollider2D polygonCollider2D;

    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float jumpVelocity = 3f;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody2D = transform.GetComponent<Rigidbody2D>();
        polygonCollider2D = transform.GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Jump") && isGrounded())
        {
            rigidBody2D.velocity = Vector2.up * jumpVelocity;
        }
        Movement();
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(polygonCollider2D.bounds.center, polygonCollider2D.bounds.size, 0f, Vector2.down, 0.1f, platformLayerMask);
        return raycastHit2D.collider != null;
    }

    private void Movement()
    {
        if (Input.GetButton("Horizontal") && Input.GetAxis("Horizontal") > 0)
        {
            rigidBody2D.velocity = new Vector2(+moveSpeed, rigidBody2D.velocity.y);
        }
        else if (Input.GetButton("Horizontal") && Input.GetAxis("Horizontal") < 0)
        {
            rigidBody2D.velocity = new Vector2(-moveSpeed, rigidBody2D.velocity.y);
        }
        else
        {
            rigidBody2D.velocity = new Vector2(0, rigidBody2D.velocity.y);
        }
    }
}
