using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject hero;
    public Transform startPosition;
    public static bool isPaused = false;

    public GameObject pauseMenuUI;
    public GameObject controlsMenuUI;

    private GameObject mRecentCheckpoint;

    void Start()
    {
        mRecentCheckpoint = new GameObject();
        if (startPosition != null)
            mRecentCheckpoint.transform.position = startPosition.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!FindObjectOfType<GameOverMenu>().IsGameOver())
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }
    }
    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void SetRecentCheckpoint(GameObject checkpoint)
    {
        mRecentCheckpoint = checkpoint;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

<<<<<<< HEAD
    public void LastCheckpoint()
    {
        if (mRecentCheckpoint != null)
        {
            hero.transform.position = mRecentCheckpoint.transform.position;
            Time.timeScale = 1f;
            isPaused = false;
            pauseMenuUI.SetActive(false);
        }
    }

    private void Pause()
=======
    public void Controls()
>>>>>>> 77d7d981aacf8f50f330e99e10f5deff10eed053
    {
        controlsMenuUI.SetActive(true);
    }

    public void Back()
    {
        controlsMenuUI.SetActive(false);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
