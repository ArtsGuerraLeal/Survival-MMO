using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenuSpawner : MonoBehaviour {
    

    public static RadialMenuSpawner instance;
    public RadialMenu menuPrefab;
    // Use this for initialization

    private void Awake()
    {
        instance = this;
    }

    public void SpawnMenu(Interactable obj)
    {
        RadialMenu newMenu = Instantiate(menuPrefab) as RadialMenu;
        newMenu.transform.SetParent(transform, false);
        newMenu.transform.position = Input.mousePosition;
        newMenu.label.text = obj.title.ToUpper();
        newMenu.SpawnButtons(obj);

    }

    

}



