using System.Collections;
using System.Collections.Generic;
using Chroma_City_Neon_Racing;
using DG.Tweening;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    [SerializeField] private float countdownTimer;
    [SerializeField] private float emissionIntensity;
    [SerializeField] private float timeToColor;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    [SerializeField] private List<Color> colors = new List<Color>();

    void Awake()
    {
        material.color = Color.red;
        material.SetColor(Shader.PropertyToID("_EmissionColor"), material.color * emissionIntensity);
    }

    public void StartCountdown()
    {
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        AudioManager.instance.PlayAfterXSeconds(SoundType.Countdown, 0.2f);
        yield return new WaitForSeconds(1f);

        int colorIndex = 0;

        while (countdownTimer > 0)
        {
            countdownTimer--;
            Sequence seq = ApplyColorSeq(colors[colorIndex]);
            colorIndex++;
            yield return seq.WaitForCompletion();
            yield return new WaitForSeconds(1f);
        }
    }

    Sequence ApplyColorSeq(Color newColor)
    {
        Sequence colorSeq = DOTween.Sequence();

        colorSeq.Append(material.DOColor(newColor, timeToColor));
        colorSeq.Join(material.DOColor(newColor * emissionIntensity, Shader.PropertyToID("_EmissionColor"), timeToColor).OnComplete(() =>
        {
            if (countdownTimer <= 0)
            {
                GameStateManager.SetGameState(GameState.Racing);
            }
        }));
        return colorSeq;
    }
}
