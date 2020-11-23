using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Transform heroPosition;
    public Image powerUpSlot;
    public Text powerUpCoolDown;
    public Image[] itemSlot = new Image[NUM_ITEM_SLOT];
    public Text[] countText = new Text[NUM_ITEM_SLOT];
    public Sprite[] powerUpSprites = new Sprite[4]; // 4 power ups

    public Sprite powerUpDefuatImage;
    public Sprite itemDefuatImage;

    private ItemManager mItemManager;
    private HeroPotionEffect mHeroPotionEffect;
    private Health mHealth;
    private WeaponGlow weaponGlow;

    private const int NUM_ITEM_SLOT = 6;
    private const int MAX_GEM_AMOUNT = 15;
    private const int MAX_POTION_AMOUNT = 3;
    private ArrayList mInventory = new ArrayList(NUM_ITEM_SLOT);
    private int[] mNumItem = new int[NUM_ITEM_SLOT];
    private float mPowerUpTimer = 0f;

    public void UseItem(int slotIndex)
    {
        // Check if inventory at slotIndex is empty, else find if the item is either a gem or potion
        if (slotIndex == -1 || (int)mInventory[slotIndex] == -1)
        {
            return;
        }

        bool isGem = mItemManager.IsGem((Item)mInventory[slotIndex]);
        bool isPotion = mItemManager.IsPotion((Item)mInventory[slotIndex]);
        if (isGem)
        {
            //Debug.Log("IsGem is true");
            if (mNumItem[slotIndex] >= 3)
            {
                GainPowerUp((Item)mInventory[slotIndex]); // get the power up based on the gem
                mNumItem[slotIndex] -= 3;
            }
        }
        else if (isPotion)
        {
            //Debug.Log("IsPotion is true");
            if (mNumItem[slotIndex] >= 1) // Potion consume at least 1 count
            {
                // if the item is health potion heal 25% of hero max health
                if ((Item)mInventory[slotIndex] == Item.HEALTH_POTION && mHealth.GetHealth() < mHealth.GetMaxHealth())
                {
                    float heal = Mathf.Min(mHealth.GetMaxHealth() / 4f + mHealth.GetHealth(), mHealth.GetMaxHealth()) - mHealth.GetHealth();
                    mHealth.GainHealth((int)heal);
                    mNumItem[slotIndex] -= 1;
                }
                else if ((Item)mInventory[slotIndex] != Item.HEALTH_POTION)
                {
                    // TODO:
                    // else, active a potion effect
                    if ((Item)mInventory[slotIndex] == Item.GOD_POTION)
                    {
                        mHealth.SetIsGod(true);
                    }
                    else if ((Item)mInventory[slotIndex] == Item.ARMOR_POTION)
                    {
                        mHeroPotionEffect.SetPotionEffect(PotionEffect.ARMOR_EFFECT);
                    }
                    else if ((Item)mInventory[slotIndex] == Item.SPEED_POTION)
                    {
                        mHeroPotionEffect.SetPotionEffect(PotionEffect.SPEED_EFFECT);
                    }
                    //SetPotionEffect(Potion effect, true);
                    mNumItem[slotIndex] -= 1;
                }
            }
        }
        else
        {
            Debug.Log("Something is wrong");
        }
        // If the item count is less than 0, drop the item from the inventory
        if (mNumItem[slotIndex] <= 0)
        {
            Drop(slotIndex);
        }
        UpdateSlotSprites();
        UpdateCountText();
    }

    // Called from picking up item
    public bool CollectItem(Item item)
    {
        bool isCollectable = false;
        // If the item is not in the inventory and the inventory is not full
        if (mInventory.Contains(item) is false)
        {
            // Check if there is open slot
            int index = CheckOpenSlot();
            if (index != -1)
            {
                // then add the item to the inventory and increase the number of that item + 1
                mInventory[index] = item;
                mNumItem[mInventory.IndexOf(item)] += 1;
                isCollectable = true;
            }
        }
        else if (mInventory.Contains(item) is true)
        {
            int index = GetSmartItemIndex(item);
            if (index != -1)
            {
                mNumItem[index] += 1;
                isCollectable = true;
            }
            else
            {
                //Debug.Log("Check open slot");
                // Check if there is open slot
                int openIndex = CheckOpenSlot();
                if (openIndex != -1)
                {
                    //Debug.Log("Found Open slot index: " + openIndex);
                    // then add the item to the inventory and increase the number of that item + 1
                    mInventory[openIndex] = item;
                    mNumItem[openIndex] += 1;
                    isCollectable = true;
                }
            }
        }
        UpdateSlotSprites();
        UpdateCountText();
        return isCollectable;
    }

    public void Drop(int slotIndex)
    {
        // Check if inventory at slotIndex is empty, else find if the item is either a gem or potion
        if (slotIndex == -1 || (int)mInventory[slotIndex] == -1)
        {
            return;
        }
        DropItem(slotIndex);
    }

    // Called from droping item from inventory
    public void DropItem(int slotIndex)
    {
        if (slotIndex != -1)
        {
            mNumItem[slotIndex] = 0;
            mInventory[slotIndex] = -1;
        }
        UpdateSlotSprites();
        UpdateCountText();
    }

    // Start is called before the first frame update
    private void Start()
    {
        mItemManager = GameObject.FindObjectOfType<ItemManager>();
        mHeroPotionEffect = GameObject.FindObjectOfType<HeroPotionEffect>();
        mHealth = GameObject.FindObjectOfType<Health>();
        weaponGlow = GameObject.FindObjectOfType<WeaponGlow>();
        InitInventory();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            DebugInventory();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            DebugRemove();
        }
        mPowerUpTimer -= Time.smoothDeltaTime;
        powerUpCoolDown.text = "" + (int)Mathf.Max(0f, mPowerUpTimer);
        UpdatePowerUpSprite();
    }

    // initial the inventory items = -1
    private void InitInventory()
    {
        for (int i = 0; i < NUM_ITEM_SLOT; i++)
        {
            mInventory.Add(-1);
        }
    }

    // return and check if any slot is open. Aka -1.
    private int CheckOpenSlot()
    {
        for (int i = 0; i < mInventory.Count; i++)
        {
            if ((int)mInventory[i] == -1)
            {
                return i;
            }
        }
        return -1;
    }

    // Smarter get index function for collect item
    private int GetSmartItemIndex(Item item)
    {
        for (int i = 0; i < mInventory.Count; i++)
        {
            bool isGem = mItemManager.IsGem((Item)mInventory[i]);
            bool isPotion = mItemManager.IsPotion((Item)mInventory[i]);
            if (((int)mInventory[i] == (int)item && isGem && mNumItem[i] < MAX_GEM_AMOUNT) ||
                ((int)mInventory[i] == (int)item && isPotion && mNumItem[i] < MAX_POTION_AMOUNT))
            {
                return i;
            }
        }
        return -1;
    }

    // Update the inventory sprite
    private void UpdateSlotSprites()
    {
        for (int i = 0; i < mInventory.Count; i++)
        {
            if ((int)mInventory[i] != -1 && mInventory[i] != null)
            {
                //Debug.Log((int)mInventory[i]);
                itemSlot[i].sprite = mItemManager.GetItemSprite((int)mInventory[i]);
            }
            else
            {
                itemSlot[i].sprite = itemDefuatImage;
            }
        }
    }

    // Update the count of the item
    private void UpdateCountText()
    {
        for (int i = 0; i < countText.Length; i++)
        {
            countText[i].text = mNumItem[i].ToString();
        }
    }

    private void GainPowerUp(Item gem)
    {
        // Set sprite
        powerUpSlot.sprite = powerUpSprites[(int)gem];

        // fire gem
        if (gem == Item.FIRE_GEM)
            weaponGlow.SetPowerUp(4);

        // poison gem
        if (gem == Item.POISON_GEM)
            weaponGlow.SetPowerUp(3);
            
        // ice gem
        if (gem == Item.ICE_GEM)
            weaponGlow.SetPowerUp(1);

        // holy gem
        if (gem == Item.HOLY_GEM)
            weaponGlow.SetPowerUp(2);

        // Set timer
        mPowerUpTimer = 30f;
    }

    private void UpdatePowerUpSprite()
    {
        if (mPowerUpTimer <= 0f)
        {
            // change weapon color
            weaponGlow.SetPowerUp(5);

            // Deactivate power up because the timer is less than 0
            powerUpSlot.sprite = powerUpDefuatImage;
        }
    }

    private void PrintInventory()
    {
        string res = "";
        foreach (var i in mInventory)
        {
            res += i.ToString() + " ";
        }
        Debug.Log(res);
        string x = "";
        foreach (int i in mNumItem)
        {
            x += i + " ";
        }
        Debug.Log(x);
    }

    private void DebugInventory()
    {
        bool test = false;
        Vector3 pos = new Vector3(heroPosition.position.x, heroPosition.position.y, heroPosition.position.z);
        Quaternion rotate = new Quaternion(0, 0, 0, 0);
        // Spawn Item
        mItemManager.DropItem(pos, Item.FIRE_GEM, rotate);
        mItemManager.DropItem(pos, Item.ICE_GEM, rotate);
        mItemManager.DropItem(pos, Item.HOLY_GEM, rotate);
        mItemManager.DropItem(pos, Item.POISON_GEM, rotate);
        mItemManager.DropItem(pos, Item.HEALTH_POTION, rotate);
        mItemManager.DropItem(pos, Item.SPEED_POTION, rotate);
        mItemManager.DropItem(pos, Item.ARMOR_POTION, rotate);
        mItemManager.DropItem(pos, Item.GOD_POTION, rotate);
        //PrintInventory();
        /*
        test = CollectItem(Item.FIRE_GEM);
        test = CollectItem(Item.FIRE_GEM);
        test = CollectItem(Item.ICE_GEM);
        test = CollectItem(Item.ICE_GEM);
        test = CollectItem(Item.ICE_GEM);

        test = CollectItem(Item.HEALTH_POTION);
        test = CollectItem(Item.POISON_GEM);
        test = CollectItem(Item.POISON_GEM);
        test = CollectItem(Item.POISON_GEM);
        test = CollectItem(Item.POISON_GEM);
        test = CollectItem(Item.GOD_POTION);
        test = CollectItem(Item.HOLY_GEM);
        test = CollectItem(Item.HOLY_GEM);
        test = CollectItem(Item.HOLY_GEM);
        */
        //PrintInventory();
    }

    private void DebugRemove()
    {
        Drop(1);
        //PrintInventory();
        Drop(3);
        //PrintInventory();
    }

}
