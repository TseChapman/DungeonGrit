using System.Collections;
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
    public bool mKeyDropped = false;

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
        int numItemDrop = Random.Range(2, 4); // between 1 to 2
        Debug.Log("Num Item Droped: " + numItemDrop);
        for (int i = 0; i < numItemDrop; i++)
        {
            float prob = Random.Range(0, 1f);
            //Debug.Log(prob);
            CheckItem(prob, targetPosition);
        }

        // Every enemy has a chance to drop the door key
        int keyDropChance = Random.Range(1, 100); // between 1 to 100

        // 40% chance for key to drop
        if (keyDropChance >= 60)
        {
            mKeyDropped = true;
        }
    }

    public void DropItem(Vector3 targetPosition, Item item, Quaternion rotation)
    {
        Instantiate(itemPrefabs[(int)item], targetPosition, rotation);
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

    private void CheckItem(float prob, Transform targetPosition)
    {
        if (prob > 0.9f)
        {
            PlaceItem(Item.ARMOR_POTION, targetPosition);
        }
        else if (prob > 0.8f)
        {
            PlaceItem(Item.SPEED_POTION, targetPosition);
        }
        else if (prob > 0.6f)
        {
            PlaceItem(Item.HEALTH_POTION, targetPosition);
        }
        else if (prob > 0.45f)
        {
            PlaceItem(Item.POISON_GEM, targetPosition);
        }
        else if (prob > 0.3f)
        {
            PlaceItem(Item.HOLY_GEM, targetPosition);
        }
        else if (prob > 0.15f)
        {
            PlaceItem(Item.ICE_GEM, targetPosition);
        }
        else
        {
            PlaceItem(Item.FIRE_GEM, targetPosition);
        }
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
