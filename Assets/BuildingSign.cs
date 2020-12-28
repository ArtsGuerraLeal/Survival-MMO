using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSign : MonoBehaviour
{

    public bool inRange;
    Transform player;
    public InventoryObject inventory;
    public GameObject buildScreen;
    public GameObject materialPrefab;
    public BuildableObject buildableObject;
    public List<MaterialsAmount> materials;
    public List<int> amounts;
    public int x;
    public int y;
    public int[] currentAmounts;
    public bool isDone = false;
    private bool isRunning = false;
    public bool isGenerated = false;
    // Start is called before the first frame update

  
    void Start()
    {
        if (buildableObject != null) {
        materials = buildableObject.Materials;
        

            if (!isGenerated) {
                currentAmounts = new int[materials.Count];
                amounts.Clear();
                for (int i = 0; i < materials.Count; i++)
                {
                   
                    amounts.Add(materials[i].amount);
                    currentAmounts[i] = 0;
                }
            }
      

        }
    }

    public void SetBuilding(BuildableObject bo) {
        if (buildableObject == null) {
        buildableObject = bo;
        }

    }

    public void SetAmounts() {
       // materials = buildableObject.Materials;
        currentAmounts = new int[materials.Count];
        amounts.Clear();
        for (int i = 0; i < materials.Count; i++)
        {
            
            amounts.Add(materials[i].amount);
            currentAmounts[i] = 0;
        }
    }

    public void SetCurrentAmounts(int[] i) {
        currentAmounts = i;
    }
    

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.gameObject.CompareTag("Player") && buildableObject != null)
        {
            buildScreen = CanvasReference.instance.buildSignScreen;


            buildScreen.GetComponentInChildren<Button>().onClick.AddListener(delegate { Build(); });

            inRange = true;
            inventory = collision.gameObject.GetComponent<Player>().inventory;
            foreach (Transform child in buildScreen.transform.GetChild(0).transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < buildableObject.Materials.Count; i++)
            {
                GameObject mat = Instantiate(materialPrefab);
                mat.GetComponentsInChildren<Image>()[1].sprite = buildableObject.Materials[i].item.uiDisplay;
                mat.GetComponentInChildren<TextMeshProUGUI>().text = currentAmounts[i] + "/" + buildableObject.Materials[i].amount.ToString();


                mat.transform.SetParent(buildScreen.transform.GetChild(0).transform, false);
            }

            buildScreen.SetActive(true);

        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && buildableObject != null)
        {
            for (int i = 0; i < currentAmounts.Length; i++) {
                if (!isDone) {
                PlacementManager.instance.AddMaterials(x, y, i, currentAmounts[i]);

                }
            }
           
            isRunning = false;
            StopCoroutine(BuildObject());
            CastBar.instance.EndCast();
            inRange = false;
            inventory = null;
            buildScreen.SetActive(false);
            
        }

    }

    public IEnumerator BuildObject()
    {
        isRunning = true;
        bool canBuild = true;
       
            for (int i = 0; i < materials.Count; i++){
                canBuild = true;
            yield return new WaitForSeconds(.2f);
            
            for (int j = currentAmounts[i]; j < materials[i].amount; j++){
                    if (inventory != null)
                    {
                        if (inventory.Container.CountItem(materials[i].item.data.Id) < 1)
                        {
                            canBuild = false;
                        }

                        if (canBuild)
                        {
                                CastBar.instance.CallCast(.98f);
                                int slot_id = inventory.Container.GetSlotId(materials[i].item.data.Id);
                                inventory.Container.RemoveItem(slot_id);
                                currentAmounts[i]++;
                                

                                foreach (Transform child in buildScreen.transform.GetChild(0).transform)
                                {
                                    Destroy(child.gameObject);
                                }

                                for (int m = 0; m < buildableObject.Materials.Count; m++)
                                {
                                    GameObject mat = Instantiate(materialPrefab);
                                    mat.GetComponentsInChildren<Image>()[1].sprite = buildableObject.Materials[m].item.uiDisplay;
                                    mat.GetComponentInChildren<TextMeshProUGUI>().text = currentAmounts[m] + "/" + buildableObject.Materials[m].amount.ToString();
                                    mat.transform.SetParent(buildScreen.transform.GetChild(0).transform, false);
                                }
                           
                        
                        yield return new WaitForSeconds(1f);
                        
                    }

                }
            }


            for (int x = 0; x < currentAmounts.Length; x++)
            {
                if (currentAmounts[i] < materials[i].amount)
                {
                    isDone = false;
                }
                else
                {
                    isDone = true;

                }

            }

        }

        if (isDone)
        {
            GameObject go = Instantiate(buildableObject.Result, new Vector3(this.transform.parent.transform.position.x, this.transform.parent.transform.position.y), Quaternion.identity);
            PlacementManager.instance.UpdateObject(x, y, buildableObject.Id);
            go.GetComponent<TemplateScript>().isBuilt = true;
            GameObject parent = this.transform.parent.gameObject;
            buildScreen.SetActive(false);
            Destroy(parent);
        }
        yield return new WaitForSeconds(.1f);
        isRunning = false;
    }


    public IEnumerator StartBuild()
    {
        isRunning = true;

        bool canBuild = true;
        
        for (int i = 0; i < materials.Count; i++)
        {

                for (int m = currentAmounts[i]; m < materials[i].amount; m++)
                {
                    if(inventory != null) { 
                        if (inventory.Container.CountItem(materials[i].item.data.Id) < 1)
                        {
                            canBuild = false;
                        }
                        if (canBuild) {
                            int slot_id = inventory.Container.GetSlotId(materials[i].item.data.Id);
                            inventory.Container.RemoveItem(slot_id);
                            currentAmounts[i]++;

                            foreach (Transform child in buildScreen.transform.GetChild(0).transform)
                            {
                                Destroy(child.gameObject);
                            }

                            for (int j = 0; j < buildableObject.Materials.Count; j++)
                            {
                                GameObject mat = Instantiate(materialPrefab);
                                mat.GetComponentsInChildren<Image>()[1].sprite = buildableObject.Materials[j].item.uiDisplay;
                                mat.GetComponentInChildren<TextMeshProUGUI>().text = currentAmounts[j] + "/" + buildableObject.Materials[j].amount.ToString();

                                mat.transform.parent = buildScreen.transform.GetChild(0).transform;
                            }
                            yield return new WaitForSeconds(1f);

                        }
                    }
                yield return new WaitForSeconds(1f);
            }

            

            for (int x = 0; x < currentAmounts.Length; x++)
            {
                if (currentAmounts[i] < materials[i].amount)
                {
                    isDone = false;
                }
                else
                {
                    isDone = true;

                }

            }
            yield return new WaitForSeconds(.1f);
        }

        if (isDone)
        {
            GameObject go = Instantiate(buildableObject.Result, new Vector3(this.transform.parent.transform.position.x, this.transform.parent.transform.position.y), Quaternion.identity);
            go.GetComponent<TemplateScript>().isBuilt = true;
            GameObject parent = this.transform.parent.gameObject;
            buildScreen.SetActive(false);
            Destroy(parent);
        }
        yield return new WaitForSeconds(.1f);

        isRunning = false;
    }

    public void Build() {

        if (!isRunning) { 
        StartCoroutine(BuildObject());
        }


    }

}
