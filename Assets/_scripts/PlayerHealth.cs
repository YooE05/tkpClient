using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int StartHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    private void Awake()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        CurrentHealth = StartHealth;
    }

    private void Start()
    {
        GameEvents.current.OnDamagedPlayer += TakeDamage;
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
        CurrentHealth--;
        if (CurrentHealth <= 0)
        {
            GameEvents.current.Death();
        }
    }

    private void OnDestroy()
    {
        GameEvents.current.OnDamagedPlayer -= TakeDamage;
    }
}