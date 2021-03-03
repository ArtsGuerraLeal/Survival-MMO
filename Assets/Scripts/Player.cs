using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public static Player instance;
    [SyncVar]
    public string playerName = "";
    [SyncVar]
    public int maxHealth = 100;
    [SyncVar]
    public int currentHealth = 100;
    [SyncVar]
    public int maxStamina = 100;
    [SyncVar]
    public int currentStamina = 100;
    [SyncVar]
    private int maxHunger = 100;
    [SyncVar]
    private int currentHunger = 100;
    [SyncVar]
    private int experiencePoints = 0;
    [SyncVar]
    private int strength = 10;
    [SyncVar]
    private int intelligence = 10;
    [SyncVar]
    private int dexterity = 10;
    [SyncVar]
    private int perception = 10;
    [SyncVar]
    private int foragingStat = 1;
    [SyncVar]
    private int craftingStat = 1;
    [SyncVar]
    private int choppingStat = 1;
    [SyncVar]
    private int miningStat = 1;
    //   private int thirst = 100;


    public InventoryObject inventory;
    public GameObject inventoryScreen;
    public GameObject equipmentScreen;
    public TextMeshPro nameTM;
    private Vector2 mousePos;
    public GameObject cameraPrefab;
    public GameObject baseTile;

    

    [Client]
    private void Awake()
    {
        instance = this;
    //    Camera.main.gameObject.SetActive(false);

    }
    [Client]
    private void Start()
    {

        if (isLocalPlayer)
        {
            GameObject go = Instantiate(cameraPrefab);
            go.GetComponent<CameraControl>().followTarget = this.gameObject;
            CmdShoot();

        }
        //inventory.Load();
        CanvasReference.instance.health.setValue(currentHealth);
        CanvasReference.instance.stamina.setValue(currentStamina);
        CanvasReference.instance.hunger.setValue(currentHunger);
        CanvasReference.instance.health.setMaxValue(maxHealth);
        CanvasReference.instance.stamina.setMaxValue(maxStamina);
        CanvasReference.instance.hunger.setMaxValue(maxHunger);
    }
    [Client]
    public void OnTriggerEnter2D(Collider2D collision)
    {
        var item = collision.GetComponent<GroundItem>();
        if (item)
        {
            Item _item = new Item(item.item);
            if (inventory.AddItem(_item, 1))
            {
                Destroy(collision.gameObject);
            }

           
        }
    }


    [Command]
    public void CmdShoot()
    {
        nameTM.text = playerName;
        RPCShoot();
    }

    [ClientRpc]
    public void RPCShoot()
    {
        nameTM.text = playerName;
    }


    [ClientCallback]
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
         //   inventory.Save();
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            inventory.Load();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryScreen.SetActive(!inventoryScreen.activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            equipmentScreen.SetActive(!equipmentScreen.activeInHierarchy);
        }
        
        if (Input.GetKeyDown(KeyCode.G) && isLocalPlayer) {
            // SetStamina(100);
            
        }

        if (Input.GetKeyDown(KeyCode.X) && isLocalPlayer)
        {
            NetworkTerrainRenderer.instance.ReloadTerrain();
        }


        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            int x = Mathf.RoundToInt(mousePos.x);
            int y = Mathf.RoundToInt(mousePos.y);

            float distance = Vector3.Distance(this.transform.position, mousePos);

            if (distance < 1)
            {
                //  StartCoroutine(Mine(x, y, 4));
                GameObject go = Instantiate(baseTile, new Vector3(x,y), Quaternion.identity);
                
            }


        }

        if (Input.GetMouseButtonDown(1))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            int x = Mathf.RoundToInt(mousePos.x);
            int y = Mathf.RoundToInt(mousePos.y);

            float distance = Vector3.Distance(this.transform.position, mousePos);

            if (distance < 1)
            {
                StartCoroutine(Mine(x, y, 2));
            }


        }

    }
    [Client]
    private IEnumerator Mine(int x, int y, int t) {
        WaitForSeconds wait = new WaitForSeconds(1f);
        CastBar.instance.CallCast(1f);
        yield return wait;
        NetworkTerrainRenderer.instance.CmdChangeTile(x, y, t);
    }
    [Client]
    private IEnumerator CreateTerrain()
    {
        WaitForSeconds wait = new WaitForSeconds(.3f);

        for (int i = 0; i < 4; i++)
        {
            World.instance.GenerateTilesLayer(0, 0, 100, 100, i, "Test");
            yield return wait;

        }
    }
    [Client]
    private void OnApplicationQuit()
    {
        inventory.Container.Items = new InventorySlot[20];
    }
    [Client]
    public int GetHealth()
    {
        return currentHealth;
    }
    [Command]
    public void SetHealth(int value) {
        currentHealth = value;
        CanvasReference.instance.health.setValue(value);
    }
    [Client]
    public int GetStamina()
    {
        return currentStamina;
    }

    [Command]
    public void SetStamina(int value)
    {
        currentStamina = value;
        CanvasReference.instance.stamina.setValue(value);
    }

    public int GetHunger()
    {
        return currentHunger;
    }

    public void SetHunger(int value)
    {
        currentHunger = value;
        CanvasReference.instance.hunger.setValue(value);
    }

    public int GetStrength() {
        return strength;
    }

    public void SetStrength(int value) {
        strength = value;
        //CanvasReference.instance.strength.setValue(value);
    }

    public int GetIntelligence()
    {
        return intelligence;
    }

    public void SetIntelligence(int value)
    {
        intelligence = value;
        //CanvasReference.instance.strength.setValue(value);
    }

    public int GetDexterity()
    {
        return dexterity;
    }

    public void SetDexterity(int value)
    {
        dexterity = value;
        //CanvasReference.instance.strength.setValue(value);
    }

    public int GetPerception()
    {
        return perception;
    }

    public void SetPerception(int value)
    {
        perception = value;
        //CanvasReference.instance.strength.setValue(value);
    }


    public int GetForagingStat()
    {
        return foragingStat;
    }

    public void SetForagingStat(int value)
    {
        foragingStat = value;
        //CanvasReference.instance.strength.setValue(value);
    }

    public int GetCraftingStat()
    {
        return craftingStat;
    }

    public void SetCraftingStat(int value)
    {
        craftingStat = value;
        //CanvasReference.instance.strength.setValue(value);
    }

    public int GetChoppingStat()
    {
        return choppingStat;
    }

    public void SetChoppingStat(int value)
    {
        choppingStat = value;
        //CanvasReference.instance.strength.setValue(value);
    }

    public int GetMiningStat()
    {
        return miningStat;
    }

    public void SetMiningStat(int value)
    {
        miningStat = value;
        //CanvasReference.instance.strength.setValue(value);
    }

    public int GetExperience()
    {
        return experiencePoints;
    }

    public void SetExperience(int value)
    {
        experiencePoints = value;
        //CanvasReference.instance.strength.setValue(value);
    }


}
