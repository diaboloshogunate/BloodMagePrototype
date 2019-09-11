using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigidbody2D))]
public class Monster : MonoBehaviour
{
    private Health health;
    private Rigidbody2D rb;
    private GameObject player;
    [SerializeField]
    private bool isChasing = true;

    void Start()
    {
        this.health = GetComponent<Health>();
        this.rb = GetComponent<Rigidbody2D>();
        this.player = FindObjectOfType<PlayerController>().gameObject;
    }

    private void Update()
    {
        if(this.health.blood <= 0)
        {
            Destroy(this.gameObject);
        }
        else if(this.isChasing)
        {
            this.rb.velocity = (this.player.transform.position - this.transform.position).normalized;
        }
        else
        {
            this.rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            this.isChasing = true;
        }
    }
}
