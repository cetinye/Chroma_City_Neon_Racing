using Chroma_City_Neon_Racing;
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
                AudioManager.instance.PlayOneShot(SoundType.CorrectPowerUp);
                player.ChangeSpeed(true);
            }
            else
            {
                AudioManager.instance.PlayOneShot(SoundType.WrongPowerUp);
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
