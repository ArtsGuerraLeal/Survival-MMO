using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{

    public InventoryObject inventory;
    public InventoryObject equipment;

    public RecipeDatabaseObject recipeDatabase;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown("f"))
        {
            bool canCraft = true;

            foreach (ItemAmount material in recipeDatabase.GetRecipe[0].Materials) {
               // Debug.Log("Material: " + material.item.name + " amount:" + inventory.Container.CountItem(material.item.data.Id));

                if(equipment.Container.CountItem(recipeDatabase.GetRecipe[0].toolRequired.data.Id) < 1){
                    canCraft = false;
                }

                if (inventory.Container.CountItem(material.item.data.Id) < material.amount)
                {
                    canCraft = false;   
                }   
            }


            if (canCraft)
            {
                foreach (ItemAmount results in recipeDatabase.GetRecipe[0].Results)
                {
                    Item _item = new Item(results.item);
                    inventory.AddItem(_item, results.amount);
                }

                foreach (ItemAmount material in recipeDatabase.GetRecipe[0].Materials)
                {
                    int cost = material.amount;

                    for (int i = 0; i < cost; i++) {
                        int slot_id = inventory.Container.GetSlotId(material.item.data.Id);
                        inventory.Container.RemoveItem(slot_id);
                    }

                }  
 
            }
            

        }
        
    }
}
