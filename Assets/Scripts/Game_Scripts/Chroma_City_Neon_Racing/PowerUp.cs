using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private Material powerUpMat;
    [SerializeField] private ParticleSystem wrongParticle;

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
                Instantiate(wrongParticle, transform.position, Quaternion.identity, transform);
            }

            this.gameObject.SetActive(false);
        }
    }
}
