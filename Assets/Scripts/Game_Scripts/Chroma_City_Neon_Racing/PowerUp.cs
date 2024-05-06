using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material powerUpMat;
    [SerializeField] private ParticleSystem wrongParticle;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            Debug.LogWarning("Player PickedUp PowerUp");

            if (powerUpMat.color == player.GetColor() || player.GetColor() == Color.white)
            {
                player.ChangeSpeed(true);
            }
            else
            {
                player.ChangeSpeed(false);
                Instantiate(wrongParticle, transform.position, Quaternion.identity, transform);
            }

            meshRenderer.enabled = false;
        }
    }
}
