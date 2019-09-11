using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class RapidShot : MonoBehaviour
{
    private Player controller;
    private Health health;
    private Walk walk;

    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private float bulletSacrifice = 1f;
    [SerializeField]
    private float fireRate = 0.25f;
    private float fireTimer = 0f;
    public bool isShooting { get { return this.controller.GetButton("Shoot"); } }

    private void Start()
    {
        this.health = GetComponent<Health>();
        this.walk = GetComponent<Walk>();
        this.controller = ReInput.players.GetPlayer(0);
    }

    public void Process()
    {
        this.fireTimer -= Time.deltaTime;
        if (this.isShooting)
        {
            if(this.fireTimer <= 0f)
            {
                this.fireTimer = this.fireRate;
                this.Fire();
            }
        }
    }

    void Fire()
    {
        this.controller.SetVibration(0, 0.1f, 0.25f);
        this.health.blood -= this.bulletSacrifice;
        GameObject obj = Instantiate(this.bullet);
        obj.transform.position = this.spawnPoint.position;
        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.direction = this.walk.aim;
    }
}
