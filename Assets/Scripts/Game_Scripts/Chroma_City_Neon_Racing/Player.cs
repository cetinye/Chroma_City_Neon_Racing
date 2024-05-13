using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Dreamteck.Splines;
using Chroma_City_Neon_Racing;
using System;

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
    private float minSpeed;
    private float maxSpeed;
    private float speedChangeAmount;
    private float speedPenatlyAmount;
    private bool isShieldActive;

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

    [Header("Shield Variables")]
    [SerializeField] private GameObject shield;
    [SerializeField] private Material shieldMaterial;
    [SerializeField] private float targetShieldFade;

    void Awake()
    {
        SetColor(Color.white, true);
    }

    void OnDisable()
    {
        GameEvents.instance.shieldPickedUp -= OnShieldPickedUp;

        SetColor(Color.white, true);
    }

    void Start()
    {
        GameEvents.instance.shieldPickedUp += OnShieldPickedUp;

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
            targetSpeed = Mathf.Min(targetSpeed, maxSpeed);
        }
        else
        {
            StartCoroutine(SlowDownRoutine());
        }

        ChangeMotorSound();
    }

    public void SetTargetSpeed(float val, bool isInstant = false)
    {
        if (isInstant)
        {
            splineFollower.followSpeed = val;
        }
        else
        {
            targetSpeed = val;
            ChangeMotorSound();
        }
    }

    public float GetTargetSpeed()
    {
        return targetSpeed;
    }

    public float GetFollowSpeed()
    {
        return splineFollower.followSpeed;
    }

    public void SetShieldState(bool val)
    {
        isShieldActive = val;

        if (val)
        {
            shieldMaterial.DOFade(0f, 0f);
            shield.SetActive(true);
            shieldMaterial.DOFade(targetShieldFade, 1f).SetEase(Ease.InOutSine);
        }
        else
        {
            shieldMaterial.DOFade(0f, 1f).SetEase(Ease.InOutSine).OnComplete(() => shield.SetActive(false));
        }
    }

    public void FlashShield()
    {
        shieldMaterial.DOFade(0f, 0.5f).SetEase(Ease.InOutSine).OnComplete(() => shieldMaterial.DOFade(targetShieldFade, 0.5f).SetEase(Ease.InOutSine));
    }

    private void OnShieldPickedUp()
    {

    }

    public bool GetShieldState()
    {
        return isShieldActive;
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

    public void SetMinMaxSpeed(float min, float max)
    {
        minSpeed = min;
        maxSpeed = max;
    }

    public void Reset()
    {
        splineFollower.followSpeed = 0;
        splineFollower.SetPercent(0);
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
        targetSpeed = tempSpeed + speedPenatlyAmount;
        targetSpeed = Mathf.Max(targetSpeed, minSpeed);
    }
}

public enum Lane
{
    Left,
    Middle,
    Right,
}
