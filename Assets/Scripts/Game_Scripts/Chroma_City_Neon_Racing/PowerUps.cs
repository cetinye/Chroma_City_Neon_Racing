using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    [SerializeField] private List<PowerUp> powerUps = new List<PowerUp>();
    [SerializeField] private Transform leftPos;
    [SerializeField] private Transform middlePos;
    [SerializeField] private Transform rightPos;
    [SerializeField] private int swapAmount;

    void Start()
    {
        for (int i = 0; i < swapAmount; i++)
        {
            Swap(powerUps[i % powerUps.Count].transform);
        }
    }

    void Swap(Transform obj)
    {
        Vector3 oldPos = obj.localPosition;
        Transform objToSwap = powerUps[UnityEngine.Random.Range(0, powerUps.Count)].transform;
        obj.localPosition = objToSwap.localPosition;
        objToSwap.localPosition = oldPos;
    }

    public void Deactivate()
    {
        foreach (PowerUp powerUp in powerUps)
        {
            powerUp.SetIsCollideable(false);
        }
    }

    public void Reset()
    {
        foreach (PowerUp powerUp in powerUps)
        {
            powerUp.Reset();
        }
    }
}
