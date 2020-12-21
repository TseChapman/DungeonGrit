using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject GameOverMenuUI;

    public float GameOverMenuDelay = 2f;
    private bool isGameOver = false;

    public void GameOver()
    {
        isGameOver = true;
        Invoke("ActivateMenu", GameOverMenuDelay);
    }

    void ActivateMenu()
    {
        GameOverMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}
