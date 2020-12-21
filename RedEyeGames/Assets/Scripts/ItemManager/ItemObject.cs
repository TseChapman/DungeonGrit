using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public Item item;
    public int amount;

    private InventoryManager mInventoryManager;

    // Start is called before the first frame update
    private void Start()
    {
        mInventoryManager = GameObject.FindObjectOfType<InventoryManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.transform.name == "Player")
        {
            Debug.Log("Collide Player");
            bool isCollectable = mInventoryManager.CollectItem(item);
            if (isCollectable)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
