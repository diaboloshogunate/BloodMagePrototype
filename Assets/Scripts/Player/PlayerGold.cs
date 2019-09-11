using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    [SerializeField]
    private int _gold = 0;
    public int gold { get => this._gold; set => this._gold = value; } 

    public bool HasGold()
    {
        return this._gold > 0;
    }

    public bool HasGold(int amt)
    {
        return this._gold > amt;
    }
}
