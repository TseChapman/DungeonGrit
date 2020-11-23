using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public bool isOpen = false;
    public bool isRandom = true;
    public Item droppedItem;
    private Animator animator;
    private ItemManager mItemManager;

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

            if (isRandom)
                mItemManager.DropRandom(transform);
            else
                mItemManager.PlaceItem(droppedItem, transform);
        }
    }
}
