using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public Sprite gui;

    virtual public void OnUse(GameObject player)
    {
        Health health = player.GetComponent<Health>();
        health.blood += 20f;
        health.recovery = 100f;
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerItems inventory = other.gameObject.GetComponent<PlayerItems>();
            if (inventory.CanPickup())
            {
                inventory.Pickup(this);
                this.gameObject.SetActive(false);
            }
        }
    }
}
