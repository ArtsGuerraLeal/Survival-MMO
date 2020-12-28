using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{

    public InventoryObject inventory;
    public InventoryObject equipment;

    public BuildableDatabaseObject recipeDatabase;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {

        if (Input.GetKeyDown("t"))
        {
            bool canBuild = true;

            foreach (MaterialsAmount material in recipeDatabase.GetBuild[0].Materials)
            {
                // Debug.Log("Material: " + material.item.name + " amount:" + inventory.Container.CountItem(material.item.data.Id));

                if (equipment.Container.CountItem(recipeDatabase.GetBuild[0].toolRequired.data.Id) < 1)
                {
                    canBuild = false;
                }

                if (inventory.Container.CountItem(material.item.data.Id) < material.amount)
                {
                    canBuild = false;
                }
            }


            if (canBuild)
            {
                Instantiate(recipeDatabase.GetBuild[0].Result, new Vector3(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y)),Quaternion.identity);

                foreach (MaterialsAmount material in recipeDatabase.GetBuild[0].Materials)
                {
                    int cost = material.amount;

                    for (int i = 0; i < cost; i++)
                    {
                        int slot_id = inventory.Container.GetSlotId(material.item.data.Id);
                        inventory.Container.RemoveItem(slot_id);
                    }

                }

            }


        }

    }
}