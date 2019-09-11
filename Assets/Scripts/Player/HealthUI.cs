using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{

    [SerializeField]
    private Health blood;
    [SerializeField]
    private Image bloodBar;
    [SerializeField]
    private Image recoveryBar;

    private void Start()
    {
        if(!this.blood)
        {
            Debug.LogError(this.gameObject + " needs health script in inspector");
        }

        if (!this.bloodBar)
        {
            Debug.LogError(this.gameObject + " needs blood bar in inspector");
        }
        
        if (!this.recoveryBar)
        {
            Debug.LogError(this.gameObject + " needs recovery bar in inspector");
        }
    }

    void Update()
    {
        this.bloodBar.fillAmount = this.blood.GetBloodPercent();
        this.recoveryBar.fillAmount = this.blood.GetRecoverPercent();
    }
}
