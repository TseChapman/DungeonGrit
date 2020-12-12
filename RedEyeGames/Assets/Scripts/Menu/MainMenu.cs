using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject controlMenuUI;
    public GameObject helpMenuUI;

    public void Controls()
    {
        controlMenuUI.SetActive(true);
    }

    public void BackControl()
    {
        controlMenuUI.SetActive(false);
    }

    public void Help()
    {
        helpMenuUI.SetActive(true);
    }

    public void BackHelp()
    {
        helpMenuUI.SetActive(false);
    }
}
