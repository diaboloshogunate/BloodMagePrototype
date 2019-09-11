using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField]
    private float dmg;
    [SerializeField]
    private float knockback;
    [SerializeField]
    private float timeLocked = 0.25f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.tag != "Player")
        {
            return;
        }

        obj.GetComponent<Health>().blood -= this.dmg;
        obj.GetComponent<Health>().recovery -= this.dmg;
        obj.GetComponent<PlayerController>().Lock(this.timeLocked);
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        rb.AddForce((obj.transform.position - this.transform.position).normalized * this.knockback, ForceMode2D.Impulse);
    }
}
