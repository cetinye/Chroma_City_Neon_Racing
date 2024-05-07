using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerUps powerUps;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material powerUpMat;
    [SerializeField] private ParticleSystem wrongParticle;
    private bool isCollideable = true;

    void OnTriggerEnter(Collider other)
    {
        if (isCollideable && GameStateManager.GetGameState() == GameState.Racing && other.TryGetComponent<Player>(out Player player))
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
            powerUps.Deactivate();
        }
    }

    public void SetIsCollideable(bool state)
    {
        isCollideable = state;
    }
}
