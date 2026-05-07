using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class LifeBlockController : MonoBehaviour
{
    
    public LifeBlock lifeblock;

    // It adds the corresponding sprite when game runs
    SpriteRenderer spriteRenderer;
    public Sprite[] sprites; // 3 - 5 - 10

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = null; // Deleting the default sprite
        switch (lifeblock.lifeAmount)
        {
            case 3:
                spriteRenderer.sprite = sprites[0];
                break;
            case 5:
                spriteRenderer.sprite = sprites[1];
                break;
            case 10:
                spriteRenderer.sprite = sprites[2];
                break;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.Instance.playerLives += lifeblock.lifeAmount;
        GameManager.Instance.UpdateLivesUI();

        AudioManager.instance.PlaySound(AudioManager.instance.lifeBlockSound);

        Destroy(gameObject);
    }
}
