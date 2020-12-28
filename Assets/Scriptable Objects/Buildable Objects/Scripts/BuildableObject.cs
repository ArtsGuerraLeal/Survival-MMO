using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MaterialsAmount
{
    public ItemObject item;
    [Range(1, 10)]
    public int amount;

}

[CreateAssetMenu(fileName = "New Buildable Object", menuName = "Inventory System/Buildable/Default")]
public class BuildableObject : ScriptableObject
{
    public int Id;
    public Sprite uiDisplay;
    public ItemObject toolRequired;
    public List<MaterialsAmount> Materials;
    public GameObject Result;

}
