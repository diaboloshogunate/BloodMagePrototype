using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class LockedDoor : MonoBehaviour
{
    [SerializeField] private Sprite shadow;
    private Collider2D collider;
    private SpriteRenderer renderer;

    private void Start()
    {
        this.collider = GetComponent<Collider2D>();
        this.renderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerKeys keys = other.gameObject.GetComponent<PlayerKeys>();
            if (keys.HasKey())
            {
                keys.keys -= 1;
                this.collider.enabled = false;
                this.renderer.sprite = this.shadow;
            }
        }
    }
}
