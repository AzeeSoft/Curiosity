using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Slider HealthBar;
    public float Health = 100;

    private float currentHealth;

    void Start()
    {
        currentHealth = Health;
    }

    public void DamageHealth(float damage)
    {
        currentHealth -= damage;
        HealthBar.value = currentHealth;
    }
}
