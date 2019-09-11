using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textmeshPro;
    [SerializeField] private PlayerGold currency;
    
    void Start()
    {
        this.textmeshPro.SetText(this.currency.gold.ToString());
    }

    private void OnGUI()
    {
        this.textmeshPro.SetText(this.currency.gold.ToString());
    }
}
