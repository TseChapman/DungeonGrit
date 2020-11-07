using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMovement : MonoBehaviour
{
    private bool jump = false;

    [SerializeField]
    private HeroController controller;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float runSpeed = 15f;
    [SerializeField]
    private float horizontalMove = 0f;

    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("IsJumping", true);
        }
        if (Input.GetButton("Fire1"))
            animator.SetBool("IsAttacking", true);
        else
            animator.SetBool("IsAttacking", false);
    }

    public void OnLanding() { animator.SetBool("IsJumping", false); }

    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
}
