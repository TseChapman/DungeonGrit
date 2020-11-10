using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject camera;
    public GameObject hero;
    public Animator animator;
    public GameObject button;
    public GameObject Text;

    private bool isClose = false;

    public void EndPrototype()
    {
        Text.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = hero.transform.position;
        pos.z = -10;
        float dist = Vector3.Distance(gameObject.transform.position, hero.transform.position);
        if (dist <= 2f)
        {
            animator.SetBool("isHeroClose", true);
            button.SetActive(true);
            isClose = true;
        }
        else
        {
            animator.SetBool("isHeroClose", false);
            button.SetActive(false);
            isClose = false;
        }
        camera.transform.position = pos;
    }
}
