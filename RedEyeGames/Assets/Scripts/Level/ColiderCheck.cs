using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ColiderCheck : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "SecretArea")
        {
            collider.transform.parent.GetComponent<TilemapRenderer>().enabled = false;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "SecretArea")
        {
            collider.transform.parent.GetComponent<TilemapRenderer>().enabled = true;
        }
    }
}
