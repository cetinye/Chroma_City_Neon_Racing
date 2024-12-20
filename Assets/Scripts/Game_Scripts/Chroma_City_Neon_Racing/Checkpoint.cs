using System.Collections.Generic;
using Chroma_City_Neon_Racing;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Material colorMat;
    [SerializeField] private float emissionIntensity;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private List<Color> colors = new List<Color>();

    public void Initialize()
    {
        colorMat = meshRenderer.materials[1];
        Invoke(nameof(DisablePowerUps), 2f);
    }

    public void SetRandomColor()
    {
        colorMat.color = colors[UnityEngine.Random.Range(0, colors.Count)];
        meshRenderer.materials[1] = colorMat;
        colorMat.SetColor(Shader.PropertyToID("_EmissionColor"), colorMat.color * emissionIntensity);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player) && GameStateManager.GetGameState() == GameState.Racing)
        {
            AudioManager.instance.PlayOneShot(SoundType.Checkpoint);
            Debug.Log("Player Passed Checkpoint");
            player.SetColor(colorMat.color);
        }
    }

    void DisablePowerUps()
    {
        Collider[] hits = Physics.OverlapBox(transform.localPosition, transform.localScale / 20f, Quaternion.identity, Physics.AllLayers, QueryTriggerInteraction.Collide);
        foreach (Collider hitObject in hits)
        {
            // Debug.LogWarning("Hit:  " + hitObject.gameObject.name);

            if (hitObject.TryGetComponent<PowerUps>(out PowerUps powerUp))
            {
                powerUp.gameObject.SetActive(false);
                // Debug.LogWarning("Disabled: " + powerUp.gameObject.name);
            }
        }
    }
}
