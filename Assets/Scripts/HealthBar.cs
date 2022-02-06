using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private Player player;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    
        spriteRenderer.sprite = sprites[player.GetMaxHealth()];
    }

    private void OnEnable()
    {
        Player.playerTookDamage += UpdateHealthBar;
    }

    private void OnDisable()
    {
        Player.playerTookDamage -= UpdateHealthBar;
    }
    private void UpdateHealthBar(int health)
    {
        spriteRenderer.sprite = sprites[health];
    }
}
