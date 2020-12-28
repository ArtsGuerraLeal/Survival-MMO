using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRenderSort : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private int sortingOrderBase =100;
    private SpriteRenderer myRenderer;
    public GameObject parent;
    [SerializeField]
    private int offset = 0;
    void Start()
    {
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (parent)
        {
            myRenderer.sortingOrder = (int)(sortingOrderBase - parent.transform.position.y - offset);

        }
        else {
            myRenderer.sortingOrder = (int)(sortingOrderBase - transform.position.y - offset);
        }
    }
}
