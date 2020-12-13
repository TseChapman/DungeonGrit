using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject controlMenuUI;

    public void Controls()
    {
        controlMenuUI.SetActive(true);
    }

    public void Back()
    {
        controlMenuUI.SetActive(false);
    }
}
