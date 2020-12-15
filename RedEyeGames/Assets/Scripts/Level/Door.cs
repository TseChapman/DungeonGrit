using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] private string scene;
    [SerializeField] private Animator animator;
    [SerializeField] private InventoryManager mInvManager;

    public void LoadScene()
    {
        if (mInvManager.mHasKey)
        {
            mInvManager.mHasKey = false;
            SceneManager.LoadScene(scene);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && mInvManager.mHasKey)
            animator.SetBool("isHeroClose", true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            animator.SetBool("isHeroClose", false);
    }
}
