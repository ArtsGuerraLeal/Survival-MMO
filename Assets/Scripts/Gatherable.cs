using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherable : MonoBehaviour
{
    public GameObject[] drops;
    public int health = 100;
    public int quality = 10;
    public ItemObject item;
    public bool destroyable;
    public GameObject spawn;
    public int spawnAmount;
    public InventoryObject inventory;
    Coroutine coroutine = null;
    public AudioSource destroySound;
    public AudioSource gatherSound;
    public Animator anim;
    public GameObject parentTile;

    //Change to gather, give specific item

    public void Chop(int number) {
        float castTime = number;
        coroutine = StartCoroutine(StartGather(ChopSuccess,castTime));
        gatherSound.Play();
        PlayerController.instance.anim.SetBool("PlayerChop", true);
       // anim.SetBool("IsShaken", true);

    }


    public void ChopSuccess() {
        
        health -= 100;
        Item _item = new Item(item);
        Debug.Log(_item.Id);
        inventory.AddItem(_item, 1);
        PlayerController.instance.anim.SetBool("PlayerChop", false);
      //  anim.SetBool("IsShaken", false);

        if (health <= 0)
        {
            if (destroyable)
            {
                for (int i = 1; i <= spawnAmount; i++)
                {

                    //   Instantiate(spawn, new Vector3(i * 2.0F, 0, 0), Quaternion.identity);
                    GameObject go = Instantiate(spawn, transform.position, transform.rotation);
                    go.transform.position = new Vector3(go.transform.position.x + (i * 1.1f), go.transform.position.y - 1, go.transform.position.z);

                }

            }
            StartCoroutine(DestroyGatherable());
           

        }
        TileRenderer.instance.ChangeGatherable(Mathf.FloorToInt(this.transform.parent.position.x), Mathf.FloorToInt(this.transform.parent.position.y),100);
    }


    public IEnumerator StartGather(Action action,float castTime) {

        CastBar.instance.CallCast(castTime);
        
        yield return new WaitForSeconds(castTime);

        action();
    }

    public IEnumerator DestroyGatherable()
    {
        destroySound.Play();
        PlayerController.instance.anim.SetBool("PlayerChop", false);
     //   anim.SetBool("IsShaken", false);
        yield return new WaitForSeconds(2f);
        this.transform.parent.gameObject.SetActive(false);
        
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inventory = collision.gameObject.GetComponent<Player>().inventory;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inventory = null;

            if (coroutine != null) { 
                StopCoroutine(coroutine);
                gatherSound.Stop();
                PlayerController.instance.anim.SetBool("PlayerChop", false);
                CastBar.instance.EndCast();

            }

        }

    }
}
