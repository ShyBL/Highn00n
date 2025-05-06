using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Exit : MonoBehaviour
{
    public UnityEvent onCanExit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out BombPlanting bombPlanting))
        {
            if (bombPlanting.allClockTowers.All(tower => tower.hasBombPlanted))
            {
                onCanExit?.Invoke();
            }
        }
    }
}