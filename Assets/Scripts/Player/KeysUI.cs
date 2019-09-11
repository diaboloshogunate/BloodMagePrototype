using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeysUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textmeshPro;
    [SerializeField] private PlayerKeys keys;
    
    void Start()
    {
        this.textmeshPro.SetText(this.keys.keys.ToString());
    }

    private void OnGUI()
    {
        this.textmeshPro.SetText(this.keys.keys.ToString());
    }
}
