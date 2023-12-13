using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int startHealth;

    public int currentHealth;

    private void Awake()
    {
        currentHealth = startHealth;
    }
    private void Start()
    {
       GameEvents.current.OnDamagedPlayer+=TakeDamage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "trap")
        {
            collision.gameObject.SetActive(false);
            GameEvents.current.TakeDamage();
        }
    }

    void TakeDamage()
    {
        currentHealth--;
        if (currentHealth <= 0)
        {
            GameEvents.current.Death();
        }
    }


    private void OnDestroy()
    {
        GameEvents.current.OnDamagedPlayer -= TakeDamage;
    }
}
