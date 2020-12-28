using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    private Interactable instance;
    public bool inRange;
    public KeyCode interactKey;
    public UnityEvent interactAction;

    public float radius = 1.4f;
    bool isFocused = false;
    bool hasInteracted = false;
    public Gatherable gatherable;
    Transform player;
    public Transform interactionTransform;

    [System.Serializable]
    public class Action
    {
        public Color color;
        public Sprite sprite;
        public string title;
        public float castTime = 1f;
        public UnityEvent buttonInteract;
        public AudioSource soundEffect;
    }
    public string title;
    public Action[] options;

    private void Start()
    {
        instance = this;
        gatherable = this.GetComponent<Gatherable>();

        if (title == "" || title == null)
        {
            title = gameObject.name;
        }
    }

    public virtual void Interact()
    {
        //meant to be overwritten
        Debug.Log("Interacting with " + transform.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange) {
            if (Input.GetKeyDown(interactKey)) {
                interactAction.Invoke();
               
            }
        }

        if (isFocused && !hasInteracted)
        {
            float distance = Vector3.Distance(player.position, transform.position);

            if (distance <= radius)
            {

                Interact();
                hasInteracted = true;
            }
        }

    }

    public void OnFocused(Transform playerTransform)
    {
        isFocused = true;
        player = playerTransform;
        hasInteracted = false;
    }

    public void Defocus()
    {
        hasInteracted = false;
        isFocused = false;
        player = null;

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void OnMouseDown()
    {
        if (inRange)
        {
            RadialMenuSpawner.instance.SpawnMenu(this);
        }

        
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inRange = true;
        }
       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inRange = false;
        }

    }

}
