﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Item
{
    FIRE_GEM = 0,
    ICE_GEM = 1,
    HOLY_GEM = 2,
    POISON_GEM = 3,
    HEALTH_POTION = 4,
    SPEED_POTION = 5,
    ARMOR_POTION = 6,
    GOD_POTION = 7,
    NUM_ITEM = 8
}

public class ItemManager : MonoBehaviour
{
    public Sprite[] itemBank = new Sprite[(int)Item.NUM_ITEM];
    public GameObject[] itemPrefabs = new GameObject[(int)Item.NUM_ITEM];

    private ArrayList mAvailableItems = new ArrayList();

    public Sprite GetItemSprite(int itemIndex)
    {
        return itemBank[itemIndex];
    }

    public bool IsGem(Item item)
    {
        return (item == Item.FIRE_GEM || item == Item.ICE_GEM || item == Item.HOLY_GEM || item == Item.POISON_GEM);
    }

    public bool IsPotion(Item item)
    {
        return (item == Item.HEALTH_POTION || item == Item.SPEED_POTION || item == Item.ARMOR_POTION || item == Item.GOD_POTION);
    }

    public void DropRandom(Transform targetPosition)
    {
        // Drop 1 or 2 item per enemy death
        int numItemDrop = Random.Range(1, 2); // between 1 to 2
        Debug.Log("Num Item Droped: " + numItemDrop);
        for (int i = 0; i < numItemDrop; i++)
        {
            int itemIndex = Random.Range(0, mAvailableItems.Count);
            PlaceItem((Item)mAvailableItems[itemIndex], targetPosition);
        }
    }

    public void PlaceItem(Item item, Transform position)
    {
        Instantiate(itemPrefabs[(int)item], position);
    }

    // Start is called before the first frame update
    private void Start()
    {
        InitAvailableItems();
    }

    // Use this function to hardcode availble item to drop from enemy
    private void InitAvailableItems()
    {
        mAvailableItems.Add(Item.FIRE_GEM);
        mAvailableItems.Add(Item.POISON_GEM);
        mAvailableItems.Add(Item.HEALTH_POTION);
        mAvailableItems.Add(Item.GOD_POTION);
        mAvailableItems.Add(Item.HOLY_GEM);
    }
}
