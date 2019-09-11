using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class RemoteBomb : MonoBehaviour
{
    private Player controller;
    private Health health;
    [SerializeField]
    private GameObject bomb;
    private Bomb placedBomb;
    [SerializeField]
    private float bombPlaceSacrifice = 5f;
    [SerializeField]
    private float bombSyncSacrifice = 1f;
    public bool isBombPlaced { get; private set; } = false;

    private void Start()
    {
        this.health = GetComponent<Health>();
        this.controller = ReInput.players.GetPlayer(0);
    }

    public void Process()
    {
        if(!this.isBombPlaced && this.controller.GetButtonDown("Bomb"))
        {
            this.isBombPlaced = true;
            this.health.blood -= this.bombPlaceSacrifice;
            GameObject obj = Instantiate(this.bomb);
            obj.transform.position = this.transform.position;
            this.placedBomb = obj.GetComponent<Bomb>();
        }
        else if(this.isBombPlaced && this.controller.GetButtonUp("Bomb"))
        {
            this.isBombPlaced = false;
            this.placedBomb.Detonate();
            this.controller.SetVibration(0, 0.5f, 0.5f);
        }
        else if(this.isBombPlaced)
        {
            this.controller.SetVibration(0, 0.1f, 0.01f);
            this.health.blood -= this.bombSyncSacrifice * Time.deltaTime;
        }
    }
}
