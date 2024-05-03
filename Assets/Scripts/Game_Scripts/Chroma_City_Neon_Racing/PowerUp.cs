using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private Material powerUpMat;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            Debug.LogWarning("Player PickedUp PowerUp");

            if (powerUpMat.color == player.GetColor())
            {
                player.ChangeSpeed(true);
            }
            else
            {
                player.ChangeSpeed(false);
            }
        }
    }
}
