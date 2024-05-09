using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Dreamteck.Splines;
using Chroma_City_Neon_Racing;

public class Player : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;

    [Header("Player Variables")]
    [SerializeField] private SplineFollower splineFollower;
    [SerializeField] private float targetSpeed;
    [SerializeField] private float lerpFactor;
    [SerializeField] private Material playerMat;
    [SerializeField] private Color playerColor;
    [SerializeField] private float colorTime;
    private float minSpeed, maxSpeed;
    private float speedChangeAmount;
    private float speedPenatlyAmount;

    [Header("Player Shake Variables")]
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeStrength;
    [SerializeField] private int shakeVibrato;

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

    void Awake()
    {
        SetColor(Color.white, true);
    }

    void Start()
    {
        targetSpeed = splineFollower.followSpeed;
    }

    void Update()
    {
        if (GameStateManager.GetGameState() == GameState.Racing)
            splineFollower.followSpeed = Mathf.Lerp(splineFollower.followSpeed, targetSpeed, lerpFactor * Time.deltaTime);
    }

    public void ChangeSpeed(bool isUp)
    {
        if (isUp)
        {
            targetSpeed += speedChangeAmount;
        }
        else
        {
            StartCoroutine(SlowDownRoutine());
        }

        Mathf.Max(targetSpeed, levelManager.MinPlayerSpeed);
        ChangeMotorSound();
    }

    public void SetTargetSpeed(float val)
    {
        targetSpeed = val;
        ChangeMotorSound();
    }

    public void SetColor(Color newColor, bool isInstant = false)
    {
        // playerMat.color = newColor;
        playerColor = newColor;

        if (isInstant)
            playerMat.color = newColor;
        else
            playerMat.DOColor(newColor, colorTime);
    }

    public Color GetColor()
    {
        return playerColor;
    }

    private void ChangeMotorSound()
    {
        if (targetSpeed <= minSpeed + 0.1f)
            AudioManager.instance.PlayMotorSound(SoundType.MotorSpeed1);
        else if (targetSpeed <= minSpeed + 0.2f)
            AudioManager.instance.PlayMotorSound(SoundType.MotorSpeed2);
        else if (targetSpeed <= minSpeed + 0.3f)
            AudioManager.instance.PlayMotorSound(SoundType.MotorSpeed3);
        else if (targetSpeed <= minSpeed + 0.4f)
            AudioManager.instance.PlayMotorSound(SoundType.MotorSpeed4);
        else if (targetSpeed <= minSpeed + 0.5f)
            AudioManager.instance.PlayMotorSound(SoundType.MotorSpeed5);
        else if (targetSpeed <= minSpeed + 0.6f)
            AudioManager.instance.PlayMotorSound(SoundType.MotorSpeed6);
        else if (targetSpeed <= minSpeed + 0.7f)
            AudioManager.instance.PlayMotorSound(SoundType.MotorSpeed7);
        else if (targetSpeed <= minSpeed + 0.8f)
            AudioManager.instance.PlayMotorSound(SoundType.MotorSpeed8);
        else if (targetSpeed <= minSpeed + 0.9f)
            AudioManager.instance.PlayMotorSound(SoundType.MotorSpeed9);
        else if (targetSpeed <= minSpeed + 1f)
            AudioManager.instance.PlayMotorSound(SoundType.MotorSpeed10);
    }

    public void SetSpeedChangeAmount(float val)
    {
        speedChangeAmount = val;
    }

    public void SetSpeedPenaltyAmount(float val)
    {
        speedPenatlyAmount = val;
    }

    #region Lane

    public void SwitchLane(bool isRightPressed)
    {
        if (GameStateManager.GetGameState() != GameState.Racing) return;

        AudioManager.instance.PlayOneShot(SoundType.LaneChange);

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

    #endregion

    IEnumerator SlowDownRoutine()
    {
        float tempSpeed = targetSpeed;
        targetSpeed = 0f;
        modelTransform.DOShakeRotation(shakeDuration, shakeStrength, shakeVibrato);
        yield return new WaitForSeconds(0.33f);
        targetSpeed = tempSpeed - speedPenatlyAmount;
        Mathf.Max(targetSpeed, 0.25f);
    }
}

public enum Lane
{
    Left,
    Middle,
    Right,
}
