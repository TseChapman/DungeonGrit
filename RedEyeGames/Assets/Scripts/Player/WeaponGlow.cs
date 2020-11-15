using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGlow : MonoBehaviour
{

    private Renderer heroRenderer;
    private int swordColor = 1;

    // Start is called before the first frame update
    void Start()
    {
        heroRenderer = this.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (swordColor == 5)
            {
                swordColor = 1;
            }
            else
            {
                swordColor++;
            }
        }
        if (swordColor == 1)
        {
            //Debug.Log("blue");
            heroRenderer.material.SetColor("_Color", Color.blue);
        }
        else if (swordColor == 2)
        {
            //Debug.Log("yellow");
            heroRenderer.material.SetColor("_Color", Color.yellow);
        }
        else if (swordColor == 3)
        {
            //Debug.Log("green");
            heroRenderer.material.SetColor("_Color", Color.green);
        }
        else if (swordColor == 4)
        {
            //Debug.Log("red");
            heroRenderer.material.SetColor("_Color", Color.red);
        }
        else if (swordColor == 5)
        {
            heroRenderer.material.SetColor("_Color", Color.white);
        }
    }

    public int getPowerUp() { return swordColor; }
}
