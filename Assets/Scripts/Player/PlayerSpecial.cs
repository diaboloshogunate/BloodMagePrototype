using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerSpecial : MonoBehaviour
{
    public enum STATE { CHARGING, CHARGED, CHANTING, LOADED, CASTING }
    public enum RUNES { GREEN = 1, RED = 5, BLUE = 17, YELLOW = 65 }

    private Player controller;
    public STATE state { get; private set; } = STATE.CHARGING;
    public RUNES[] runes = new RUNES[4];
    private int chant;
    private int chantIndex;
    [SerializeField]
    private float _current;
    public float current { set => this._current = Mathf.Clamp(value, 0, 100); get => this._current; }
    private float max = 100f;
    private float timeToChant = 5f;
    private float chantTimeRemaining;
    public GameObject canvas;

    public float GetMeterPercent()
    {
        return Mathf.Clamp01(this.current / this.max);
    }
    
    public float GetTimerPercent()
    {
        return Mathf.Clamp01(this.chantTimeRemaining / this.timeToChant);
    }
    
    private void Start()
    {
        this.controller = ReInput.players.GetPlayer(0);
    }

    private void Update()
    {
        switch (this.state)
        {
            case STATE.CHARGING:
                if (this.current == 100f)
                {
                    this.state = STATE.CHARGED;
                    goto case STATE.CHARGED;
                }
                break;
            case STATE.CHARGED:
                this.controller.SetVibration(0, 0.25f, 1f);
                if (this.controller.GetButtonDown("Special"))
                {
                    this.OpenChantWindow();
                    this.state = STATE.CHANTING;
                    goto case STATE.CHANTING;
                }
                break;
            case STATE.CHANTING:
                this.controller.SetVibration(0, 0.5f, 1f);
                this.Chanting();
                this.chantTimeRemaining -= Time.unscaledDeltaTime;
                if (chantIndex == 4 || this.chantTimeRemaining <= 0f)
                {
                    this.state = STATE.LOADED;
                    this.CloseChantWindow();
                }
                break;
            case STATE.LOADED:
                this.controller.SetVibration(0, 0.75f, 1f);
                if (this.controller.GetButtonUp("Special"))
                {
                    this.state = STATE.CASTING;
                    this.Cast();
                }
                break;
            case STATE.CASTING:
                this.controller.SetVibration(0, 1f, 1f);
                this.Casting();
                this.state = STATE.CHARGING;
                break;
        }
    }

    private void OpenChantWindow()
    {
        Time.timeScale = 0;
        this.chant = 0;
        this.chantIndex = 0;
        this.runes = new RUNES[4];
        this.current = 0;
        this.canvas.SetActive(true);
        this.chantTimeRemaining = this.timeToChant;
    }

    private void CloseChantWindow()
    {
        this.canvas.SetActive(false);
        Time.timeScale = 1;
    }

    private void Chanting()
    {
        if (this.chantIndex < this.runes.Length)
        {
            if (this.controller.GetButtonDown("Rune1"))
            {
                this.AddChant(RUNES.GREEN);
            }
            else if (this.controller.GetButtonDown("Rune2"))
            {
                this.AddChant(RUNES.RED);
            }
            else if (this.controller.GetButtonDown("Rune3"))
            {
                this.AddChant(RUNES.BLUE);
            }
            else if (this.controller.GetButtonDown("Rune4"))
            {
                this.AddChant(RUNES.YELLOW);
            }
        }
    }

    private void AddChant(RUNES rune)
    {
        this.chant += (int) rune;
        this.runes[this.chantIndex] = rune;
        this.chantIndex++;
    }

    private void Cast()
    {
        Debug.Log("Casting spell " + this.chant);
    }

    private void Casting()
    {
        
    }
}
