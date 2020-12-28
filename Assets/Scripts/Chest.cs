using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Chest : MonoBehaviour
{
    
    public InventoryObject inv;
    public Inventory inventory;
    //public InventoryObject pinv;

    public ItemDatabaseObject db;
    public GameObject screenPrefab;
    public GameObject screen;

    public GameObject prefab;
    public DynamicInterface chestUi;
    public ItemObject free;
    public GameObject canvas;
    public int rand;
    // Start is called before the first frame update
    void Start()
    {
        
        rand = Random.Range(1, 100);
        InventoryObject myInventory = ScriptableObject.CreateInstance("InventoryObject") as InventoryObject;
        //myInventory.Init("Chest Inventory " + rand, "/" + rand + "chest.save", db);
        //InventoryObject myInventory = new InventoryObject();
        
        inv = myInventory;
        inv.name = "Chest Inventory " + rand;
        inv.database = db;
        inv.savePath = "/"+ rand + "chest.save";
        
        inv.Container = new Inventory();
        inventory = inv.Container;
        inv.Container.Items = new InventorySlot[20];
       //AssetDatabase.CreateAsset(myInventory, "Assets/"+rand + ".asset");
       // EditorUtility.SetDirty(myInventory);
        //AssetDatabase.SaveAssets();

        // inv.AddItem(free.data, 1);

        //   Debug.Log("INV length " + inv.Container.Items.Length);

        //   Debug.Log("INV Parent " + inv.Container.Items);
        //   Debug.Log("INV Parent " + inv.Container.Items[0].item.Id);

        //foreach (InventorySlot item in myInventory.Container.Items) {
        //    item.item = new Item();
        //    Debug.Log("INV Parent " + item.item);

        //}



        /*
                DynamicInterface dynamicInterface = screen.AddComponent<DynamicInterface>();
                dynamicInterface.enabled = false;
                dynamicInterface.inventory = myInventory;
                dynamicInterface.inventoryPrefab = prefab;
                dynamicInterface.X_START = -86;
                dynamicInterface.Y_START = 128;
                dynamicInterface.X_SPACE_BETWEEN_ITEM = 57;
                dynamicInterface.NUMBER_OF_COLUMN = 4;
                dynamicInterface.Y_SPACE_BETWEEN_ITEMS = 60;

            */

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            GameObject chest = Instantiate(screenPrefab, canvas.transform);
            screen = chest;
            RectTransform chestRect = chest.GetComponent<RectTransform>();
            chest.SetActive(true);
          //  chest.transform.position = new Vector3(950, 50, 0);
            chestRect.localPosition = new Vector3(350, 50, 0);
            chestUi = screen.GetComponent<DynamicInterface>();
            chestUi.inventory = inv;
           // chestUi.UpdateSlots();

            // inventory = collision.gameObject.GetComponent<Player>().inventory;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(screen);
            screen = null;
            chestUi = null;
           // chestUi.UnsetSlots();
            // inventory = collision.gameObject.GetComponent<Player>().inventory;
        }

    }
}
