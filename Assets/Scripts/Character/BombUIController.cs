using System;
using UnityEngine;

public class BombUIController : MonoBehaviour
{
    [SerializeField] private GameObject bombIcon;

    private void Awake()
    {
        bombIcon.SetActive(false);
    }
    
    public void ShowBombIcon()
    {
        bombIcon.SetActive(true);
    }
    
    public void HideBombIcon()
    {
        bombIcon.SetActive(false);
    }
}