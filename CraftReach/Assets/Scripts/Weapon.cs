using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    public float speed = 15f, life = 3f, cooldown = 1f, ammo = 10f;

    public GameObject bullet;
    public Transform firePos;

    override public string ToString()
    {
        string info = "";

        info += "speed: " + speed;
        info += "\nlife: " + life;
        info += "\ncooldown" + cooldown;
        info += "\nammo" + ammo;

        return info;
    }
}

