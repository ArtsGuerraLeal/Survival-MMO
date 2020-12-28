using UnityEngine;
using System.Collections;

public class TemplateScript : MonoBehaviour
{

    [SerializeField]
    private GameObject finalObject;

    public BuildableObject buildableObject;

 

    private Vector2 mousePos;

    [SerializeField]
    private LayerMask allTilesLayer;

    public bool isBuilt = false;



    // Update is called once per frame
    void Update()
    {
        if (!isBuilt)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector2(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y));
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D rayHit = Physics2D.Raycast(transform.position, Vector2.zero, Mathf.Infinity, allTilesLayer);
                if (rayHit.collider == null)
                {
                    GameObject go = Instantiate(finalObject, transform.position, Quaternion.identity);
                    go.GetComponentInChildren<BuildingSign>().buildableObject = buildableObject;
                    go.GetComponentInChildren<BuildingSign>().x = (int)transform.position.x;
                    go.GetComponentInChildren<BuildingSign>().y = (int)transform.position.y;
                    Debug.Log(buildableObject.name);
                    PlacementManager.instance.CreateSign((int)transform.position.x, (int)transform.position.y, buildableObject.Id);

                    PlacementScript.instance.PlacedObject();
                    
                }
                //int x = Mathf.FloorToInt(mousePos.x);
                //int y = Mathf.FloorToInt(mousePos.y);
                //Debug.Log(TileRenderer.instance.GetTile(x, y).GetComponent<BaseTile>().type);

                /*
                    if (rayHit.collider != null)
                    {
                    Debug.Log("HIT");

                    if (rayHit.collider.gameObject.tag == "TerrainTile" && this.gameObject.tag == "BuildingTemplate")
                    {
                        Instantiate(finalObject, transform.position, Quaternion.identity);
                    }
                }
                else if (rayHit.collider == null && this.gameObject.tag == "TerrainTile")
                {
                    Instantiate(finalObject, transform.position, Quaternion.identity);
                }
                */
            }
        }
    }
}