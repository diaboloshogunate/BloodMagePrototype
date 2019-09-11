using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class PlayerItems : MonoBehaviour
{
    private Player controller;
    [SerializeField] private Sprite noItemGui;
    [SerializeField] private Item item1;
    [SerializeField] private Item item2;
    [SerializeField] private Item item3;
    [SerializeField] private Item item4;

    private void Start()
    {
        this.controller = ReInput.players.GetPlayer(0);
    }

    public void Update()
    {
        if (this.controller.GetButtonDown("Item1") && this.item1 != null)
        {
            this.item1.OnUse(this.gameObject);
        }
        if (this.controller.GetButtonDown("Item2") && this.item2 != null)
        {
            this.item2.OnUse(this.gameObject);
        }
        if (this.controller.GetButtonDown("Item3") && this.item3 != null)
        {
            this.item3.OnUse(this.gameObject);
        }
        if (this.controller.GetButtonDown("Item4") && this.item4 != null)
        {
            this.item4.OnUse(this.gameObject);
        }
    }

    public bool CanPickup()
    {
        return (this.item1 == null || this.item2 == null || this.item3 == null || this.item4 == null);
    }

    public void Pickup(Item item)
    {
        if (this.item1 == null)
        {
            this.item1 = item;
        }
        else if (this.item2 == null)
        {
            this.item2 = item;
        }
        else if (this.item3 == null)
        {
            this.item3 = item;
        }
        else if (this.item4 == null)
        {
            this.item4 = item;
        }
    }

    public Sprite GetGuiItem(int position)
    {
        switch (position)
        {
            case 1:
                if (this.item1 != null)
                {
                    return this.item1.gui;
                }
                break;
            case 2:
                if (this.item2 != null)
                {
                    return this.item2.gui;
                }
                break;
            case 3:
                if (this.item3 != null)
                {
                    return this.item3.gui;
                }
                break;
            case 4:
                if (this.item4 != null)
                {
                    return this.item4.gui;
                }
                break;
            default:
                return this.noItemGui;
        }

        return this.noItemGui;
    }
}
