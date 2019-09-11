using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Walk))]
[RequireComponent(typeof(Rigidbody2D))]
public class Dodge : MonoBehaviour
{
    private Player controller;
    private Health health;
    private Walk walk;
    private Rigidbody2D rb;
    private AudioSource audio;
    private Animator anim;
    private Vector2 movement;

    [SerializeField]
    private float dodgeVelocity = 20f;
    [SerializeField]
    private float dodgeTime = 0.25f;
    private float dodgeTimer = 0f;
    [SerializeField]
    private float dodgeCooldown = 1f;
    private float dodgeCooldownTimer = 0f;
    [SerializeField]
    private float dodgeSacrifice;
    [SerializeField]
    private AudioClip startDodge;
    [SerializeField]
    private AudioClip endDodge;
    [SerializeField]
    private LayerMask dodgeMask;    

    public bool isDodging
    {
        get { return this.dodgeTimer > 0f; }
    }
    public bool canDodg
    {
        get { return this.dodgeCooldownTimer > 0f; }
    }

    private void Start()
    {
        this.controller = ReInput.players.GetPlayer(0);
        this.health = GetComponent<Health>();
        this.walk = GetComponent<Walk>();
        this.rb = GetComponent<Rigidbody2D>();
        this.audio = GetComponent<AudioSource>();
        this.anim = GetComponent<Animator>();
    }

    public void Process()
    {
        if(this.isDodging)
        {
            this.dodgeTimer = Mathf.Clamp(this.dodgeTimer - Time.deltaTime, 0, this.dodgeTime);
            if (this.dodgeTimer > 0f && this.controller.GetButton("Dodge"))
            {
                ContinueDodge();
            }
            else
            {
                this.dodgeTimer = 0f;
                this.dodgeCooldownTimer = this.dodgeCooldown;
                this.EndDodge();
            }
        }
        else
        {
            this.dodgeCooldownTimer = Mathf.Clamp(this.dodgeCooldownTimer - Time.deltaTime, 0, this.dodgeCooldown);
            if (this.dodgeCooldownTimer <= 0f && this.controller.GetButtonDown("Dodge"))
            {
                this.dodgeTimer = this.dodgeTime;
                this.StartDodge();
            }
        }
    }

    void StartDodge()
    {
        this.health.blood -= this.dodgeSacrifice;
        this.rb.velocity = this.walk.direction * this.dodgeVelocity;
        this.audio.PlayOneShot(this.startDodge);
        this.anim.SetBool("IsDodging", true);
        this.controller.SetVibration(0, 0.2f, 0.1f);
        for (int i = 0; i < 32; i++)
        {
            if((this.dodgeMask.value & (1 << i)) > 0f){
                Physics2D.IgnoreLayerCollision(this.gameObject.layer, i, true);
            }
        }
    }

    void ContinueDodge()
    {
        this.movement.x = this.controller.GetAxis("MoveHorizontal");
        this.movement.y = this.controller.GetAxis("MoveVertical");
        this.controller.SetVibration(0, 0.01f, 0.01f);
        if(this.movement.sqrMagnitude > 0)
        {
            this.rb.velocity = this.movement * this.dodgeVelocity;
        }

        return;
    }

    void EndDodge()
    {
        this.audio.PlayOneShot(this.endDodge);
        this.anim.SetBool("IsDodging", false);
        this.controller.SetVibration(0, 0.3f, 0.2f);
        for (int i = 0; i < 32; i++)
        {
            if ((this.dodgeMask.value & (1 << i)) > 0f)
            {
                Physics2D.IgnoreLayerCollision(this.gameObject.layer, i, false);
            }
        }
        return;
    }
}
