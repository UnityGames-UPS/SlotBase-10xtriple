using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BonusController : MonoBehaviour
{
  [SerializeField] private RectTransform Wheel_Transform;
  [SerializeField] private RectTransform Wheel;

  [Header("Wheel Segments")]
  [SerializeField] private List<string> segments = new List<string>();
  [Tooltip("Offset in degrees to align segment[0] with the pointer at 0° rotation — tune after wheel art arrives")]
  [SerializeField] private float startOffsetAngle = 0f;

  [SerializeField] private TMP_Text WinAmountText;
  [SerializeField] private GameObject Bonus_Object;
  [SerializeField] private SlotBehaviour slotManager;
  [SerializeField] private AudioManager _audioManager;
  [SerializeField] private GameObject PopupPanel;
  [SerializeField] private Transform Win_Transform;
  [SerializeField] private Transform Boost_Transform;

  [SerializeField] private UIManager uIManager;

  [Header("Wheel Entrance")]
  [SerializeField] private float wheelBigScale = 1.5f;
  [SerializeField] private float wheelSmallScale = 0.7f;
  [SerializeField] private float wheelFinalScale = 1.2f;
  [SerializeField] private Vector2 wheelFinalAnchoredPos = new Vector2(0f, -400f);
  [SerializeField] private float entranceAppearDuration = 0.7f;
  [SerializeField] private float entranceScaleDownDuration = 0.5f;
  [SerializeField] private float entranceMoveDuration = 0.8f;
  [SerializeField] private float entranceScaleUpDuration = 0.6f;
  [SerializeField] private float spinDuration = 3f;
  [SerializeField] private float spinUpDuration = 1.5f;
  [SerializeField] private float spinLoopDuration = 1f;
  [SerializeField] private int stopExtraRotations = 2;
  [SerializeField] private float stopDuration = 2.5f;
  [SerializeField] private GameObject GreenPinataOnWheel;
  [SerializeField] private GameObject WheelBonusPanel;

  internal bool isBonusDone = false;

  private Tween wheelRoutine;
  private Coroutine _spinCoroutine;

  internal void StartWheelBonus(List<string> spinHistory, double winAmount)
  {
    isBonusDone = false;
    if (PopupPanel) PopupPanel.SetActive(false);
    if (Win_Transform) Win_Transform.gameObject.SetActive(false);
    if (Boost_Transform) Boost_Transform.gameObject.SetActive(false);
    if (WinAmountText) WinAmountText.text = winAmount.ToString("F3");
    if (_audioManager) _audioManager.PlayBonusBgMusic();
    if (GreenPinataOnWheel) GreenPinataOnWheel.SetActive(false);
    if (WheelBonusPanel) { WheelBonusPanel.SetActive(false); WheelBonusPanel.transform.localScale = Vector3.one; }
    if (Bonus_Object) Bonus_Object.SetActive(true);
    StartCoroutine(WheelEntranceRoutine(spinHistory));
  }

  private IEnumerator WheelEntranceRoutine(List<string> spinHistory)
  {
    if (Wheel)
    {
      Wheel.localEulerAngles = Vector3.zero;
      Wheel.anchoredPosition = Vector2.zero;
      Wheel.localScale = Vector3.zero;
    }

    RotateWheel();

    // Phase 1: Appear big at center — spinning
    yield return Wheel.DOScale(wheelBigScale, entranceAppearDuration).SetEase(Ease.OutBack).WaitForCompletion();

    // Phase 2: Scale down at center — still spinning
    yield return Wheel.DOScale(wheelSmallScale, entranceScaleDownDuration).SetEase(Ease.InOutCubic).WaitForCompletion();

    // Stop spinning before moving down
    if (_spinCoroutine != null) { StopCoroutine(_spinCoroutine); _spinCoroutine = null; }

    // Phase 3: Move to bottom; green pinata appears
    if (GreenPinataOnWheel) GreenPinataOnWheel.SetActive(true);
    yield return Wheel.DOAnchorPos(wheelFinalAnchoredPos, entranceMoveDuration).SetEase(Ease.InOutCubic).WaitForCompletion();

    // Phase 4: Wheel scales up + WheelBonusPanel pops in simultaneously
    if (WheelBonusPanel) { WheelBonusPanel.SetActive(true); WheelBonusPanel.transform.localScale = Vector3.zero; WheelBonusPanel.transform.DOScale(Vector3.one, entranceScaleUpDuration).SetEase(Ease.OutBack); }
    yield return Wheel.DOScale(wheelFinalScale, entranceScaleUpDuration).SetEase(Ease.OutBack).WaitForCompletion();

    yield return new WaitForSeconds(0.3f);
    yield return StartCoroutine(SpinSequenceRoutine(spinHistory));
  }

  private IEnumerator SpinSequenceRoutine(List<string> spinHistory)
  {
    for (int i = 0; i < spinHistory.Count; i++)
    {
      string spin = spinHistory[i];
      bool isBoost = spin == "boost";
      bool isLast = i == spinHistory.Count - 1;

      int segIndex = GetSegmentIndex(spin);
      RotateWheel();
      yield return new WaitForSeconds(spinDuration);
      yield return StartCoroutine(StopWheelRoutine(segIndex));

      if (isBoost)
      {
        if (Boost_Transform) { Boost_Transform.gameObject.SetActive(true); Boost_Transform.localScale = Vector3.zero; Boost_Transform.DOScale(Vector3.one, 0.5f); }
        if (PopupPanel) PopupPanel.SetActive(true);
        PlayWinLooseSound(false);
        yield return new WaitForSeconds(1.5f);
        if (PopupPanel) PopupPanel.SetActive(false);
        if (Boost_Transform) Boost_Transform.gameObject.SetActive(false);
      }
      else
      {
        if (Win_Transform) { Win_Transform.gameObject.SetActive(true); Win_Transform.localScale = Vector3.zero; Win_Transform.DOScale(Vector3.one, 1f); }
        if (PopupPanel) PopupPanel.SetActive(true);
        PlayWinLooseSound(true);
      }
    }

    yield return new WaitForSeconds(1.5f);
    if (_audioManager) _audioManager.StopBonusBgMusic();
    if (Wheel) { Wheel.anchoredPosition = Vector2.zero; Wheel.localScale = Vector3.one; }
    isBonusDone = true;
    if (Bonus_Object) Bonus_Object.SetActive(false);
  }

  private IEnumerator StopWheelRoutine(int segIndex)
  {
    if (_spinCoroutine != null) { StopCoroutine(_spinCoroutine); _spinCoroutine = null; }
    if (wheelRoutine != null) wheelRoutine.Kill();

    float segAngle = segments.Count > 0 ? 360f / segments.Count : 60f;
    float segmentCenter = segIndex * segAngle + segAngle / 2f;
    float finalTarget = (segmentCenter + startOffsetAngle) % 360f;

    float spinSpeed = 360f / spinLoopDuration;
    float currentZ = Wheel_Transform.localEulerAngles.z;
    float additionalRotation = (currentZ - finalTarget + 360f) % 360f;
    float totalToRotate = stopExtraRotations * 360f + additionalRotation;

    // Constant deceleration — covers spinSpeed*stopDuration/2 degrees in stopDuration seconds
    float decelerationDistance = spinSpeed * stopDuration / 2f;
    float linearDistance = totalToRotate - decelerationDistance;
    float decel = spinSpeed / stopDuration;

    if (linearDistance < 0f)
    {
      linearDistance = 0f;
      decel = spinSpeed * spinSpeed / (2f * totalToRotate);
    }


    // Phase 1: constant speed
    float rotated = 0f;
    while (rotated < linearDistance)
    {
      float delta = Mathf.Min(spinSpeed * Time.deltaTime, linearDistance - rotated);
      Wheel_Transform.Rotate(0, 0, -delta);
      rotated += delta;
      yield return null;
    }

    // Phase 2: constant deceleration
    float velocity = spinSpeed;
    while (velocity > 0f && rotated < totalToRotate)
    {
      velocity = Mathf.Max(0f, velocity - decel * Time.deltaTime);
      float delta = Mathf.Min(velocity * Time.deltaTime, totalToRotate - rotated);
      if (delta <= 0f) break;
      Wheel_Transform.Rotate(0, 0, -delta);
      rotated += delta;
      yield return null;
    }

    // Snap to exact position
    float finalZ = Wheel_Transform.localEulerAngles.z;
    float snapDelta = Mathf.DeltaAngle(finalZ, finalTarget);
    if (Mathf.Abs(snapDelta) > 0.01f)
      Wheel_Transform.Rotate(0, 0, snapDelta);
  }

  internal int GetSegmentIndex(string segmentType)
  {
    List<int> matches = new List<int>();
    for (int i = 0; i < segments.Count; i++)
    {
      if (segments[i] == segmentType) matches.Add(i);
    }
    if (matches.Count == 0)
    {
      Debug.LogError($"Segment '{segmentType}' not found in wheel segments list!");
      return 0;
    }
    return matches[Random.Range(0, matches.Count)];
  }

  private void RotateWheel()
  {
    if (_spinCoroutine != null) StopCoroutine(_spinCoroutine);
    _spinCoroutine = StartCoroutine(SpinUpRoutine());
    if (_audioManager) _audioManager.PlaySpinLoop();
  }

  private IEnumerator SpinUpRoutine()
  {
    float targetVelocity = 360f / spinLoopDuration;
    float acceleration = targetVelocity / spinUpDuration;
    float velocity = 0f;

    while (velocity < targetVelocity)
    {
      velocity = Mathf.MoveTowards(velocity, targetVelocity, acceleration * Time.deltaTime);
      if (Wheel_Transform) Wheel_Transform.Rotate(0, 0, -velocity * Time.deltaTime);
      yield return null;
    }

    while (true)
    {
      if (Wheel_Transform) Wheel_Transform.Rotate(0, 0, -targetVelocity * Time.deltaTime);
      yield return null;
    }
  }

  internal void PlayWinLooseSound(bool isWin)
  {
    if (isWin)
    {
      if (_audioManager) _audioManager.PlayBigWin();
    }
    else
    {
      if (_audioManager) _audioManager.PlayFreeGameBonus();
    }
  }
}
