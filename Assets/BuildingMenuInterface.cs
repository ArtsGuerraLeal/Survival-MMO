using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuInterface : MonoBehaviour
{
    public GameObject buildingSlotPrefab;
    public GameObject materialSlotPrefab;
    public TextMeshProUGUI objectText;
    public GameObject content;
    public GameObject materials;
    public BuildableDatabaseObject buildables;
    public Dictionary<GameObject, BuildableObject> slotsOnInterface = new Dictionary<GameObject, BuildableObject>();
    public GameObject buildingSelected;

    private void Start()
    {
       

        for (int i = 0; i < buildables.Builds.Length; i++)
        {
            GameObject slot = Instantiate(buildingSlotPrefab);
            slot.name = "Button " + i;

            slot.GetComponentsInChildren<Image>()[1].sprite = buildables.Builds[i].uiDisplay;

            slot.GetComponentInChildren<Button>().onClick.AddListener(delegate { SetActiveBuildable(slot); });
            
           // slot.transform.parent = content.transform;
            slot.transform.SetParent(content.transform, false);
            slotsOnInterface.Add(slot, buildables.Builds[i]);
        }
    }

    public void SetActiveBuildable(GameObject button) {
        buildingSelected = button;
        objectText.text = slotsOnInterface[button].name;
        foreach (Transform child in materials.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < slotsOnInterface[button].Materials.Count; i++)
        {
            GameObject mat = Instantiate(materialSlotPrefab);
            mat.GetComponentsInChildren<Image>()[1].sprite = slotsOnInterface[button].Materials[i].item.uiDisplay;
            mat.GetComponentInChildren<TextMeshProUGUI>().text = "x" + slotsOnInterface[button].Materials[i].amount.ToString();
            mat.transform.SetParent(materials.transform, false);

            //mat.transform.parent = materials.transform;
        }
    }

    public void PlaceBuild() {
        if (buildingSelected != null) {
            PlacementScript.instance.ChooseObject(slotsOnInterface[buildingSelected].Result, slotsOnInterface[buildingSelected]);
        }

    }

}
