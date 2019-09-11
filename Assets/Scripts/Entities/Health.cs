using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// blood is the actually health
// the entity can recover up to the recover amount
// if the recover amount is reduced to a value lower than the blood then the blood is reduces as well
public class Health : MonoBehaviour
{
    private float max = 100f;
    
    [SerializeField] 
    private float _blood = 100f;
    public float blood
    {
        get => this._blood;
        set
        {
            if (this.special && this._blood > value)
            {
                special.current += (this._blood - value);
            }
            this._blood = Mathf.Clamp(value, 0, Mathf.Min(this.max, this.recovery));
        }
    }

    [SerializeField] 
    private float _recovery = 100f;
    public float recovery { 
        get => this._recovery;
        set
        {
            this._recovery = Mathf.Clamp(value, 0, this.max);
            if (this._recovery < this.blood)
            {
                this.blood = this._recovery;
            }
        }
    }
    public float regen  { get; set; } = 1f;
    [SerializeField]
    private PlayerSpecial special; // @TODO move to player special script and hook into this one

    public bool HasBlood()
    {
        return this.blood > 0f;
    }

    public bool HasBlood(float amt)
    {
        return this.blood > amt;
    }

    public bool CanRecover()
    {
        return this.blood < this.recovery;
    }

    public void Recover()
    {
        this.blood += this.regen * Time.deltaTime;
    }

    public float GetBloodPercent()
    {
        return this.blood / this.max;
    }

    public float GetRecoverPercent()
    {
        return this.recovery / this.max;
    }
}