using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private GameObject gameOver;

    private void Update()
    {
        if (health == 0)
        {
            gameObject.SetActive(true);
            Destroy(gameObject);
        }
    }
    public void TakeDamage()
    {
        health--;
    }
}
