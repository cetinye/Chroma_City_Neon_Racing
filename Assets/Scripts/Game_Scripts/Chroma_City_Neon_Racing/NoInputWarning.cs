using DG.Tweening;
using TMPro;
using UnityEngine;

public class NoInputWarning : MonoBehaviour
{
    [SerializeField] float timeToWaitForWarning;
    [SerializeField] TMP_Text warningTMPText;
    float timeSinceLastTouch = 0f;
    bool warned = false;

    void Awake()
    {
        TextFade(0f, 0f);
    }

    void Update()
    {
        if (Input.touchCount == 0)
        {
            timeSinceLastTouch += Time.deltaTime;

            if (timeSinceLastTouch >= timeToWaitForWarning && !warned)
            {
                warned = true;
                Debug.LogWarning("No touch detected for 5 seconds!");
                TextFade(1f, 1f);
            }
        }
        else
        {
            timeSinceLastTouch = 0f;
            warned = false;
            TextFade(0f, 1f);
        }
    }

    Tween TextFade(float targetFade, float timeToFade)
    {
        return warningTMPText.DOFade(targetFade, timeToFade);
    }
}
