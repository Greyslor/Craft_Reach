using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public int damage = 40;

    private void OnTriggerEnter(Collider other)
    {
        if (!isOwned) return; // Solo el que dispara manda la colision

        if (other.CompareTag("Player"))
        {
            NetworkIdentity identity = other.GetComponent<NetworkIdentity>();
            if (identity != null)
            {
                CmdHitPlayer(identity, damage);
            }
        }

        CmdDestroyBullet(); // Pide al servidor destruir la bala
    }

    [Command]
    void CmdHitPlayer(NetworkIdentity target, int damage)
    {
        NetworkPlayer player = target.GetComponent<NetworkPlayer>();
        if (player != null)
        {
            player.CmdTakeDamage(damage);
        }
    }

    [Command]
    void CmdDestroyBullet()
    {
        NetworkServer.Destroy(gameObject);
    }
}


