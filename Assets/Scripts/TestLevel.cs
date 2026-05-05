using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevel : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    void Start()
    {
        gameManager.playerLives += 979;
    }
}
