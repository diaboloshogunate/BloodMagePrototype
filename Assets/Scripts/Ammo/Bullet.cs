using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField]
    private float dmg = 1f;
    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private Vector2 _direction = new Vector2(1, 1);
    public Vector2 direction
    {
        get { return this._direction;  }
        set
        {
            this._direction = value;
            this.transform.rotation = Quaternion.LookRotation(Vector3.forward, this._direction);
            if(!this.rb) { this.rb = GetComponent<Rigidbody2D>(); }
            this.rb.velocity = this._direction.normalized * this.speed;
        }
    }

    [SerializeField]
    private float _lifetime = 2f;
    public float lifetime {
        get { return this._lifetime; }
        set { this._lifetime = value; }
    }
    private float lifetimerTimer;

    void Start()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.lifetimerTimer = this.lifetime;
        this.direction = this._direction;
    }

    void Update()
    {
        this.lifetimerTimer -= Time.deltaTime;
        if(this.lifetimerTimer <= 0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Health health = collision.gameObject.GetComponent<Health>();
        if(health)
        {
            health.blood -= this.dmg;
        }

        Destroy(this.gameObject);
    }
}
