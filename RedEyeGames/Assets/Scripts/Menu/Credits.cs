using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    LevelLoader levelLoader;
    [SerializeField] string level;

    private void Start()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            levelLoader.LoadNextLevel(level);
    }

    public void Level(string level)
    {
        levelLoader.LoadNextLevel(level);
    }
}
