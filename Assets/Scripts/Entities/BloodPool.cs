using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodPool : MonoBehaviour
{
    [SerializeField]
    private float amt = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Health blood = other.gameObject.GetComponent<Health>();
            blood.recovery += this.amt;
            Destroy(this.gameObject);
        }
    }
}
