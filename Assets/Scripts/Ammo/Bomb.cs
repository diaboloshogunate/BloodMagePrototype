using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Bomb : MonoBehaviour
{
    [SerializeField]
    private float dmg = 5f;
    private bool isDoingDmg = false;
    private Animator anim;

    private void Start()
    {
        this.anim = GetComponent<Animator>();
    }
    public void Detonate()
    {
        this.anim.SetTrigger("Detonate");
        this.isDoingDmg = true;
    }

    public void StartDmg()
    {

    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.isDoingDmg)
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if (health)
            {
                health.blood -= this.dmg;
            }
        }
    }
}
