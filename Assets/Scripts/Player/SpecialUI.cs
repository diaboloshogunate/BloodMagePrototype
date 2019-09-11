using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialUI : MonoBehaviour
{
    public Image meter;
    public GameObject canvas;
    public PlayerSpecial special;
    public Image rune1;
    public Image rune2;
    public Image rune3;
    public Image rune4;
    public Sprite emptyRune;
    public Sprite greenRune;
    public Sprite redRune;
    public Sprite blueRune;
    public Sprite yellowRune;
    public Image Timer;

    private void OnGUI()
    {
        this.meter.fillAmount = this.special.GetMeterPercent();
        if (this.canvas.activeSelf)
        {
            this.Timer.fillAmount = this.special.GetTimerPercent();
            this.rune1.overrideSprite = this.GetRune(0);
            this.rune2.overrideSprite = this.GetRune(1);
            this.rune3.overrideSprite = this.GetRune(2);
            this.rune4.overrideSprite = this.GetRune(3);
        }
    }

    private Sprite GetRune(int position)
    {
        switch (this.special.runes[position])
        {
            case PlayerSpecial.RUNES.GREEN:
                return this.greenRune;
            case PlayerSpecial.RUNES.RED:
                return this.redRune;
            case PlayerSpecial.RUNES.BLUE:
                return this.blueRune;
            case PlayerSpecial.RUNES.YELLOW:
                return this.yellowRune;
            default:
                return this.emptyRune;
        }
    }
}
