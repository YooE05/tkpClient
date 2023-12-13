using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }

    public event Action<Vector2> OnExitTriggerEnter;
    public void ExitTriggerEnter(Vector2 direction)
    {
        if(OnExitTriggerEnter!=null)
        {
            OnExitTriggerEnter(direction);
        }
    }

    public event Action OnDamagedPlayer;
    public void TakeDamage()
    {
        if (OnDamagedPlayer != null)
        {
            OnDamagedPlayer();
        }
    }

    public event Action OnIncreasePoints;
    public void IncreasePoints()
    {
        if (OnIncreasePoints != null)
        {
            OnIncreasePoints();
        }
    }

    public event Action OnPlayerDied;
    public void Death()
    {
        if (OnPlayerDied != null)
        {
            OnPlayerDied();
        }
    }

}
