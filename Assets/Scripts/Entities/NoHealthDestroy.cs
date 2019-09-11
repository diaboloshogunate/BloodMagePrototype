using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class NoHealthDestroy : MonoBehaviour
{
    private Health health;

    private void Start()
    {
        this.health = GetComponent<Health>();
    }

    void Update()
    {
        if (this.health.blood <= 0f)
        {
            Destroy(this.gameObject);
        }
    }
}
