using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] float timeToColor;
    [SerializeField] float emissionIntensity;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            GameStateManager.SetGameState(GameState.Success);
        }
    }

    public void StartLightChange()
    {
        LightChange(material, timeToColor, emissionIntensity);
    }

    Sequence LightChange(Material material, float timeToColor, float emissionIntensity)
    {
        Sequence seq = DOTween.Sequence();

        Color[] colors = new Color[] { Color.black };
        int colorIndex = 0;

        seq.SetLoops(-1, LoopType.Yoyo);

        seq.Append(material.DOColor(colors[colorIndex], timeToColor).OnComplete(() =>
        {
            colorIndex = (colorIndex + 1) % colors.Length;
        }));
        seq.Join(material.DOColor(colors[colorIndex] * emissionIntensity, Shader.PropertyToID("_EmissionColor"), timeToColor));

        return seq;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        material.color = Color.white;
        material.SetColor(Shader.PropertyToID("_EmissionColor"), material.color * emissionIntensity);
    }

}

