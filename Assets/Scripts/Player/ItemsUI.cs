using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsUI : MonoBehaviour
{
    [SerializeField] private PlayerItems playerItems;
    [SerializeField] private Image item1;
    [SerializeField] private Image item2;
    [SerializeField] private Image item3;
    [SerializeField] private Image item4;

    private void OnGUI()
    {
        this.item1.overrideSprite = playerItems.GetGuiItem(1);
        this.item2.overrideSprite = playerItems.GetGuiItem(2);
        this.item3.overrideSprite = playerItems.GetGuiItem(3);
        this.item4.overrideSprite = playerItems.GetGuiItem(4);
    }
}
