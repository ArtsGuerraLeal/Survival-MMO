using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemAmount
{
    public ItemObject item;
    [Range(1,10)]
    public int amount;
    
}

[CreateAssetMenu(fileName = "New Recipe Object", menuName = "Inventory System/Recipe/Default")]
public class RecipeObject : ScriptableObject
{
    public int Id;
    public ItemObject toolRequired;
    public List<ItemAmount> Materials;
    public List<ItemAmount> Results;

   

}
