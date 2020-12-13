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
<<<<<<< HEAD
        // Save data


        // Load Scene
=======
        mInvManager.mHasKey = false;
>>>>>>> d6c7183b215a5bd8347216c093b30ebf8492f455
        SceneManager.LoadScene(scene);
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
