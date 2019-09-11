using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootTable : MonoBehaviour
{
    public Loot[] items;

    private void OnDestroy()
    {
        foreach (Loot item in this.items)
        {
            float number = Random.Range(0f, 100f);
            if (number < item.chance)
            {
                GameObject obj = Instantiate(item.item);
                obj.transform.position = this.transform.position;
            }
        }
    }
}
