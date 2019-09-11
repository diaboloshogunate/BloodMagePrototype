using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeys : MonoBehaviour
{
    [SerializeField]
    private int _keys = 0;
    public int keys { get => this._keys; set => this._keys = value; }

    public bool HasKey()
    {
        return this._keys > 0;
    }
}
