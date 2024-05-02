using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Material colorMat;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private List<Color> colors = new List<Color>();

    void Awake()
    {
        colorMat = meshRenderer.materials[1];
    }

    public void SetRandomColor()
    {
        colorMat.color = colors[UnityEngine.Random.Range(0, colors.Count)];
        meshRenderer.materials[1] = colorMat;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            Debug.LogWarning("Player Passed");
            player.SetColor(colorMat.color);
        }
    }
}