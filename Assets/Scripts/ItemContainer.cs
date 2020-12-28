using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{

    //public bool generated = false;
    //public bool open;
    //public ItemObject[] itemsInChest;
    //public List<Item> generatedItems = new List<Item>();
    //public InventoryObject chestData;
    //public DynamicInterface chestUi;

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        open = true;
    //        UpdateChestData();
    //        chestUi.gameObject.SetActive(true);
    //        // inventory = collision.gameObject.GetComponent<Player>().inventory;
    //    }

    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        open = false;
    //        chestUi.slotsOnInterface.Clear();
    //        chestData.Clear();
    //        foreach (Transform child in chestUi.transform)
    //        {
    //            GameObject.Destroy(child.gameObject);
    //        }
    //        chestUi.gameObject.SetActive(false);
    //        // inventory = collision.gameObject.GetComponent<Player>().inventory;
    //    }

    //}

    //private void UpdateChestData()
    //{
    //    if (generated)
    //    {
    //        for (int i = 0; i < generatedItems.Count; i++)
    //        {
    //            chestData.AddItem(generatedItems[i], 1);
    //        }
    //        return;
    //    }
    //    for (int i = 0; i < itemsInChest.Length; i++)
    //    {
    //        Item item = new Item(itemsInChest[i]);
    //        chestData.AddItem(item, 1);
    //        generatedItems.Add(item);
    //    }
    //    generated = true;
    //}
    //private void Update()
    //{
    //    if (!chestUi.isActiveAndEnabled || !open)
    //        return;
    //    for (int i = 0; i < generatedItems.Count; i++)
    //    {
    //        bool found = false;
    //        for (int j = 0; j < chestData.Container.Items.Length; j++)
    //        {
    //            if (generatedItems[i] == chestData.Container.Items[j].item)
    //            {
    //                found = true;
    //            }
    //        }
    //        if (found == false)
    //        {
    //            generatedItems.Remove(generatedItems[i]);
    //        }
    //    }

    //}
    //private void OnApplicationQuit()
    //{
    //    chestData.Clear();
    //}



    //public InventoryObject inv;
    //public InventoryObject pinv;

    //public ItemDatabaseObject db;
    //public GameObject screen;
    //public GameObject prefab;
    //// Start is called before the first frame update
    //void Start()
    //{
    //    InventoryObject myInventory = ScriptableObject.CreateInstance("InventoryObject") as InventoryObject;
    //    inv = myInventory;

    //    myInventory.name = "Chest Inventory";
    //    myInventory.database = db;
    //    myInventory.savePath = "/chest.save";

    //    myInventory.Container = new Inventory();
    //    myInventory.Container.Items = new InventorySlot[20];

    //    Debug.Log("INV length " + myInventory.Container.Items.Length);

    //    Debug.Log("INV Parent " + myInventory.Container.Items);
    //    Debug.Log("INV Parent " + myInventory.Container.Items[0].item.Id);
    //    //foreach (InventorySlot item in myInventory.Container.Items) {
    //    //    item.item = new Item();
    //    //    Debug.Log("INV Parent " + item.item);

    //    //}



    //    /*
    //            DynamicInterface dynamicInterface = screen.AddComponent<DynamicInterface>();
    //            dynamicInterface.enabled = false;
    //            dynamicInterface.inventory = myInventory;
    //            dynamicInterface.inventoryPrefab = prefab;
    //            dynamicInterface.X_START = -86;
    //            dynamicInterface.Y_START = 128;
    //            dynamicInterface.X_SPACE_BETWEEN_ITEM = 57;
    //            dynamicInterface.NUMBER_OF_COLUMN = 4;
    //            dynamicInterface.Y_SPACE_BETWEEN_ITEMS = 60;

    //        */

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        screen.SetActive(true);
    //       // inventory = collision.gameObject.GetComponent<Player>().inventory;
    //    }

    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        screen.SetActive(false);
    //        // inventory = collision.gameObject.GetComponent<Player>().inventory;
    //    }

    //}
}
