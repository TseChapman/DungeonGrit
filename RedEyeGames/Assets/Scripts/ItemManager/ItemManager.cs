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

    public void PlaceItem(Item item, Transform position)
    {
        Instantiate(itemPrefabs[(int)item], position);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
