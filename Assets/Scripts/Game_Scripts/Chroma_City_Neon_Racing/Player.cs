using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [Header("Lane Switch Variables")]
    [SerializeField] private Transform modelTransform;
    [SerializeField] private float leftLaneX;
    [SerializeField] private float middleLaneX;
    [SerializeField] private float rightLaneX;
    [SerializeField] private float laneSwitchTime;
    [SerializeField] private Vector3 rotationAmount;
    [SerializeField] private float rotationTime;
    Sequence laneSwitchSeq;
    private Lane currentLane = Lane.Middle;

    public void SwitchLane(bool isRightPressed)
    {
        //lane conversion
        if (currentLane == Lane.Left)
        {
            if (isRightPressed)
                currentLane = Lane.Middle;
            else
                return;
        }
        else if (currentLane == Lane.Middle)
        {
            if (isRightPressed)
                currentLane = Lane.Right;
            else
                currentLane = Lane.Left;
        }
        else if (currentLane == Lane.Right)
        {
            if (isRightPressed)
                return;
            else
                currentLane = Lane.Middle;
        }

        //rotation conversion
        if (isRightPressed)
        {
            rotationAmount = new Vector3(rotationAmount.x, rotationAmount.y, -Mathf.Abs(rotationAmount.z));
        }
        else
        {
            rotationAmount = new Vector3(rotationAmount.x, rotationAmount.y, Mathf.Abs(rotationAmount.z));
        }

        SwitchLaneSequence();
    }

    void SwitchLaneSequence()
    {
        laneSwitchSeq = DOTween.Sequence();

        laneSwitchSeq.Append(transform.DOLocalMoveX(GetLanePosition(), laneSwitchTime));
        laneSwitchSeq.Join(modelTransform.DOLocalRotate(rotationAmount, rotationTime).OnComplete(() =>
        {
            modelTransform.DOLocalRotate(Vector3.zero, rotationTime);
        }));

        laneSwitchSeq.Play();
    }

    float GetLanePosition()
    {
        return currentLane switch
        {
            Lane.Left => leftLaneX,
            Lane.Middle => middleLaneX,
            Lane.Right => rightLaneX,
            _ => middleLaneX,
        };
    }
}

public enum Lane
{
    Left,
    Middle,
    Right,
}
