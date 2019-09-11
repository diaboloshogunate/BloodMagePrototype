using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Rewired;
using UnityEngine.Serialization;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigidbody2D))]
public class Walk : MonoBehaviour
{
    public Vector2 direction { get; private set; } // last movement
    public Vector2 aim { get; private set; } // current aim

    [SerializeField] private float walkVelocity = 5f;
    [SerializeField] private bool faceMovement = true;
    [SerializeField] private GameObject aimCursor;
    [SerializeField] private Transform CameraOffsetTarget;
    [SerializeField] private Vector2 cameraOffsetBounds;
    [SerializeField] private float cameraOffsetSpeed = 0.025f;
    
    private Player controller;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movement; // current movement
    private float velocity; // current velocity
    public Vector2 cameraOffset;

    public bool isMoving
    {
        get { return this.movement.sqrMagnitude > 0f; }
    }

    void Start()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.anim = GetComponent<Animator>();
        this.controller = ReInput.players.GetPlayer(0);
        this.movement = new Vector2(0, 1);
        this.aim = new Vector2(0, 1);
    }

    public void Process()
    {
        this.Move();
        this.Aim();
    }

    void Move()
    {
        this.movement.x = this.controller.GetAxis("MoveHorizontal");
        this.movement.y = this.controller.GetAxis("MoveVertical");
        this.velocity = 0f;

        if (this.movement.sqrMagnitude > 0f)
        {
            this.anim.SetFloat("MoveX", this.movement.x);
            this.anim.SetFloat("MoveY", this.movement.y);
            this.direction = this.movement;
            this.velocity = this.walkVelocity;
        }


        this.rb.velocity = this.movement * this.velocity;
        this.anim.SetFloat("Velocity", this.rb.velocity.sqrMagnitude);
    }

    void Aim()
    {
        Vector2 aim = new Vector2();
        if(this.faceMovement)
        {
            aim = this.movement;
        }
        else
        {
            aim.x = this.controller.GetAxis("AimHorizontal");
            aim.y = this.controller.GetAxis("AimVertical");
            this.cameraOffset.x = Mathf.Clamp(this.cameraOffset.x + aim.x * this.cameraOffsetSpeed * Time.deltaTime, this.cameraOffsetBounds.x * -1, this.cameraOffsetBounds.x);
            this.cameraOffset.y = Mathf.Clamp(this.cameraOffset.y + aim.y * this.cameraOffsetSpeed * Time.deltaTime, this.cameraOffsetBounds.y * -1, this.cameraOffsetBounds.y);
            this.CameraOffsetTarget.position = this.transform.position + (Vector3) this.cameraOffset;
        }

        if(aim.sqrMagnitude > 0f)
        {
            this.aim = aim;
            this.aimCursor.transform.position = this.transform.position + (Vector3) this.aim.normalized * 1f;
            this.aimCursor.transform.rotation = Quaternion.LookRotation(Vector3.forward, this.aim);
        }
    }
}
