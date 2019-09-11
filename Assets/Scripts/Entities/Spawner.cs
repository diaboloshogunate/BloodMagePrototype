using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class Spawner : MonoBehaviour
{
    private bool isPlayerInRange = false;
    [SerializeField]
    private float rate = 2f;
    private float nextWave = 0;
    [SerializeField]
    private GameObject monster;
    [SerializeField]
    private float spawnRadius = 2f;
    private Health health;

    private void Start()
    {
        this.health = GetComponent<Health>();
    }

    void Update()
    {
        if(this.health.blood <= 0f)
        {
            Destroy(this.gameObject);
            return;
        }

        if (this.isPlayerInRange)
        {
            this.nextWave -= Time.deltaTime;
            if (this.nextWave <= 0f)
            {
                this.nextWave = this.rate;
                GameObject obj = Instantiate(this.monster);
                obj.transform.position = this.transform.position +
                                         new Vector3(Random.Range(this.spawnRadius * -1f, this.spawnRadius),
                                             Random.Range(this.spawnRadius * -1f, this.spawnRadius));
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            this.isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            this.isPlayerInRange = false;
        }
    }
}
