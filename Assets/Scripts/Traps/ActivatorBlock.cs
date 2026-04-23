using System;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorBlock : MonoBehaviour
{
    public enum ActivatorType
    {
        Pink,
        Blue,
        Yellow
    }

    [Header("Type")]
    public ActivatorType type;

    [Header("Sprites")]
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private Sprite idleSprite;

    private SpriteRenderer sr;

    private readonly HashSet<Collider2D> contacts = new();

    public static event Action<ActivatorType, bool> OnActivatorChanged;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        SetVisual(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Enemy") && !other.CompareTag("Platform"))
            return;

        contacts.Add(other);
        UpdateState();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Enemy") && !other.CompareTag("Platform"))
            return;

        contacts.Remove(other);
        UpdateState();
    }

    private void Update()
    {
        // Clean invalid colliders (important for player state changes)
        contacts.RemoveWhere(c => c == null || !c.enabled);
    }

    private void UpdateState()
    {
        bool pressed = contacts.Count > 0;

        SetVisual(pressed);
        OnActivatorChanged?.Invoke(type, pressed);
    }

    private void SetVisual(bool pressed)
    {
        if (sr != null)
            sr.sprite = pressed ? pressedSprite : idleSprite;
    }
}