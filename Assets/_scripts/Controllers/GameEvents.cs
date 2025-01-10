using UnityEngine;
using System;
using Mirror;

public class GameEvents : NetworkBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }

    public event Action<Vector2> OnExitTriggerEnter;

    public void ExitTriggerEnter(Vector2 direction)
    {
        OnExitTriggerEnter?.Invoke(direction);
    }

    public event Action OnDamagedPlayer;

    public void TakeDamage()
    {
        OnDamagedPlayer?.Invoke();
    }

    public event Action OnIncreasePoints;

    public void IncreasePoints()
    {
        OnIncreasePoints?.Invoke();
    }

    public event Action OnPlayerDied;

    public void Death()
    {
        OnPlayerDied?.Invoke();
    }
}