using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buildable Database", menuName = "Inventory System/Buildable/Database")]
public class BuildableDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public BuildableObject[] Builds;
    public Dictionary<int, BuildableObject> GetBuild = new Dictionary<int, BuildableObject>();

    public void OnAfterDeserialize()
    {
        for (int i = 0; i < Builds.Length; i++)
        {
            Builds[i].Id = i;
            GetBuild.Add(i, Builds[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        GetBuild = new Dictionary<int, BuildableObject>();
    }
}