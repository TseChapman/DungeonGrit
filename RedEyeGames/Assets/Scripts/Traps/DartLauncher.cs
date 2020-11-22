using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DartLauncher : MonoBehaviour
{
    [SerializeField] private GameObject dart;
    [SerializeField] private float rateOfFire = 1f;
    private float lastShot = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (lastShot + rateOfFire < Time.time)
            {
                if (GetComponent<SpriteRenderer>().flipX)
                    Instantiate(dart, transform.position, Quaternion.AngleAxis(180, Vector3.up));
                else
                    Instantiate(dart, transform.position, Quaternion.identity);
                lastShot = Time.time;
            }
        }
    }
}
