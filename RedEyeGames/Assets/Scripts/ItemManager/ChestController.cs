using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    private Animator animator;
    private ItemManager mItemManager;

    public int numberItems = 1;
    public bool isRandom = true;
    public Item droppedItem;
    private bool isOpen = false;

    public void Start()
    {
        animator = GetComponent<Animator>();
        mItemManager = GameObject.FindObjectOfType<ItemManager>();
    }

    public void OpenChest()
    {
        if (!isOpen)
        {
            isOpen = true;
            animator.SetBool("IsOpen", isOpen);

            for (int i = 0; i < numberItems; i++)
            {
                if (isRandom)
                    mItemManager.DropRandom(transform);
                else
                    mItemManager.PlaceItem(droppedItem, transform);
            }
        }
    }
}
