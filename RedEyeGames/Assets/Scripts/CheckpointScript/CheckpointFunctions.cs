using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointFunctions : MonoBehaviour
{
    public GameObject unActiveRune;
    public GameObject activeRune;

    private PauseMenu mPauseMenu;

    // Start is called before the first frame update
    private void Start()
    {
        mPauseMenu = GameObject.FindObjectOfType<PauseMenu>();
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.transform.name == "Player")
        {
            unActiveRune.SetActive(false);
            activeRune.SetActive(true);
            mPauseMenu.SetRecentCheckpoint(this.gameObject);
        }
    }
}
