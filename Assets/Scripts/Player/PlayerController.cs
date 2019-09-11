using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public enum STATE { DEFAULT, DODGE, DEAD, LOCKED }
    public STATE state { get; set; } = STATE.DEFAULT;
    private float timeUntilUnlock;

    private Rigidbody2D rb;
    private Animator anim;
    private Health health;
    private Recover recover;
    private Walk walk;
    private Dodge dodge;
    private RapidShot rapid;
    private RemoteBomb bomb;

    void Start()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.anim = GetComponent<Animator>();
        this.health = GetComponent<Health>();
        this.recover = GetComponent<Recover>();
        this.walk = GetComponent<Walk>();
        this.dodge = GetComponent<Dodge>();
        this.rapid = GetComponent<RapidShot>();
        this.bomb = GetComponent<RemoteBomb>();
    }

    void Update()
    {
        if (this.state != STATE.DEAD && !this.health.HasBlood())
        {
            this.state = STATE.DEAD;
            this.rb.velocity = Vector2.zero;
            this.anim.SetTrigger("IsDead");
        }

        switch (this.state)
        {
            case STATE.LOCKED:
                this.timeUntilUnlock -= Time.deltaTime;
                if(this.timeUntilUnlock <= 0f)
                {
                    this.Unlock();
                    goto case STATE.DEFAULT;
                }
                return;
            case STATE.DEFAULT:
                this.dodge.Process();
                if(this.dodge.isDodging)
                {
                    this.state = STATE.DODGE;
                    return;
                }

                this.walk.Process();
                this.rapid.Process();
                this.bomb.Process();
                this.recover.isMoving = this.walk.isMoving;
                this.recover.isUsingAbilities = this.rapid.isShooting || this.bomb.isBombPlaced;
                this.recover.Process();
                break;
            case STATE.DODGE:
                this.dodge.Process();
                if (!this.dodge.isDodging)
                {
                    this.state = STATE.DEFAULT;
                    goto case STATE.DEFAULT;
                }
                break;
            case STATE.DEAD:
                SceneManager.LoadScene("GameOver");
                break;
        }
    }

    public void Lock(float time)
    {
        this.timeUntilUnlock = time;
        this.state = STATE.LOCKED;
    }

    public void Unlock()
    {
        this.timeUntilUnlock = 0;
        this.state = default;
    }
}
