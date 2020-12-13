using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class InventorySave : MonoBehaviour
{
    InventoryManager inventoryManager;

    private const int NUM_OF_INV = 6;

    //public string[] typeOfItem = new string[NUM_OF_INV]
    public ArrayList mInventory = new ArrayList(NUM_OF_INV); // type of item

    public int[] numOfItems = new int[NUM_OF_INV];

    public int health;



    public static InventorySave Instance;


    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //inventoryManager = GameObject.FindObjectOfType<InventoryManager>();

        /*NUM_OF_INV = inventoryManager.*/
    }

    // get the Inventory
    public void SaveInventory(ArrayList itemTypes, int[] numOfItem)
    {
        for (int i = 0; i < NUM_OF_INV; i++)
        {
            InventorySave.Instance.mInventory[i] = itemTypes[i];
            InventorySave.Instance.numOfItems[i] = numOfItem[i];
        }
    }

    // these two functions are to set the inventory
    public ArrayList SetInventoryTypes()
    {
        return InventorySave.Instance.mInventory;
    }

    public int[] SetInventoryCount()
    {
        return InventorySave.Instance.numOfItems;
    }

    public void SaveHealth(int health) 
    {
        InventorySave.Instance.health = health;
    }

    public int SetHealth()
    {
        return InventorySave.Instance.health;
    }

    // Is called when Player Restarts game, (maybe also when player dies)
    public void ClearPlayerInfo()
    {
        Destroy(gameObject);
    }

    /*public static InventorySave invSave;

    public List<ScriptableObject> objects = new List<ScriptableObject>();

    *//*private const int NUM_ITEM_SLOT = 6;

    private ArrayList mInventory = new ArrayList(NUM_ITEM_SLOT); 

    private int[] mNumItem = new int[NUM_ITEM_SLOT];*//*

    private void Awake()
    {
        if(invSave == null)
        {
            invSave = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    *//*public void SaveScriptables()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            FileStream file = File.Create(Application.persistentDataPath + string.Format("/{0}.dat", i));

            BinaryFormatter binary = new BinaryFormatter();
            var json = JsonUtility.ToJson(objects[i]);

            binary.Serialize(file, json);

            // Close file
            file.Close();
        }
    }

    public LoadScriptables()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            if(File.Exists(Application.persistentDataPath + 
                    string.Format("/{0}.dat", i)))
            {
                FileStream file = File.Open(Application.persistentDataPath + 
                    string.Format("/{0}.dat", i)), FileMode.Open)
            }

        }
    }*/
}
