using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Recover : MonoBehaviour
{
    private Health health;
    private Walk walk;

    private float rate;
    [SerializeField]
    private float noAbilityRate = 1f;
    [SerializeField]
    private float noMovementRate = 2f;
    public bool isMoving { get; set; } = false;
    public bool isUsingAbilities { get; set; } = false;

    void Start()
    {
        this.health = GetComponent<Health>();
        this.walk = GetComponent<Walk>();
    }

    public void Process()
    {
        this.rate = 0f;
        if(!this.isUsingAbilities)
        {
            this.rate = this.isMoving ? this.noAbilityRate : this.noMovementRate;
        }

        this.health.blood += this.rate * Time.deltaTime;
    }
}
