using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class OrientationChange : MonoBehaviour
{
  [SerializeField] private RectTransform UIWrapper;
  [SerializeField] private CanvasScaler CanvasScaler;
  [SerializeField] private float MatchWidth = 0f;
  [SerializeField] private float MatchHeight = 1f;
  [SerializeField] private float PortraitMatchWandH = 0.5f;
  [SerializeField] private float transitionDuration = 0.2f;
  [SerializeField] private float waitForRotation = 0.2f;

  private Vector2 ReferenceAspect;
  private Tween matchTween;
  private Tween rotationTween;
  private Coroutine rotationRoutine;
  private bool isLandscape;
  private void Awake()
  {
    ReferenceAspect = CanvasScaler.referenceResolution;
  }

  void SwitchDisplay(string dimensions)
  {
    if (rotationRoutine != null) StopCoroutine(rotationRoutine);
    rotationRoutine = StartCoroutine(RotationCoroutine(dimensions));
  }

  IEnumerator RotationCoroutine(string dimensions)
  {
    yield return new WaitForSecondsRealtime(waitForRotation);
    string[] parts = dimensions.Split(',');
    if (parts.Length == 2 && int.TryParse(parts[0], out int width) && int.TryParse(parts[1], out int height) && width > 0 && height > 0)
    {
      Debug.Log($"Unity: Received Dimensions - Width: {width}, Height: {height}");

      isLandscape = width > height;

      Quaternion targetRotation = isLandscape ? Quaternion.identity : Quaternion.Euler(0, 0, -90);
      if (rotationTween != null && rotationTween.IsActive()) rotationTween.Kill();
      rotationTween = UIWrapper.DOLocalRotateQuaternion(targetRotation, transitionDuration).SetEase(Ease.OutCubic);

      float currentAspectRatio = isLandscape ? (float)width / height : (float)height / width;
      float referenceAspectRatio = ReferenceAspect.x / ReferenceAspect.y;

      float targetMatch = isLandscape ? (currentAspectRatio > referenceAspectRatio ? MatchHeight : MatchWidth) : PortraitMatchWandH;
      float ratio = (float)width / height;
      const float e = 0.0001f;
      if (Mathf.Abs(ratio - (float)2340/1080) < e) targetMatch = 1.0f;
      else if (Mathf.Abs(ratio - (float)1080/2340) < e) targetMatch = 0.43f;
      else if (Mathf.Abs(ratio - (float)1080/1920) < e) targetMatch = 0.503f;
      else if (Mathf.Abs(ratio - (float)1920/1080) < e) targetMatch = 0.5f;
      else if (Mathf.Abs(ratio - (float)375/667) < e) targetMatch = 0.503f;
      else if (Mathf.Abs(ratio - (float)667/375) < e) targetMatch = 0.46f;
      else if (Mathf.Abs(ratio - (float)853/1280) < e) targetMatch = 0.412f;
      else if (Mathf.Abs(ratio - (float)1280/853) < e) targetMatch = 0.0f;
      else if (Mathf.Abs(ratio - (float)768/1024) < e) targetMatch = 0.333f;
      else if (Mathf.Abs(ratio - (float)1024/768) < e) targetMatch = 0.0f;
      else if (Mathf.Abs(ratio - (float)820/1180) < e) targetMatch = 0.39f;
      else if (Mathf.Abs(ratio - (float)1180/820) < e) targetMatch = 0.0f;
      else if (Mathf.Abs(ratio - (float)1366/1024) < e) targetMatch = 0.0f;
      else if (Mathf.Abs(ratio - (float)1024/1366) < e) targetMatch = 0.333f;
      else if (Mathf.Abs(ratio - (float)344/882) < e) targetMatch = 0.388f;
      else if (Mathf.Abs(ratio - (float)882/344) < e) targetMatch = 1.0f;
      else if (Mathf.Abs(ratio - (float)390/844) < e) targetMatch = 0.424f;
      else if (Mathf.Abs(ratio - (float)844/390) < e) targetMatch = 1.0f;
      else if (Mathf.Abs(ratio - (float)1080/2400) < e) targetMatch = 0.421f;
      else if (Mathf.Abs(ratio - (float)2400/1080) < e) targetMatch = 1.0f;
      else if (Mathf.Abs(ratio - (float)2304/1440) < e) targetMatch = 0.0f;
      else if (Mathf.Abs(ratio - (float)1440/2304) < e) targetMatch = 0.45f;
      else if (Mathf.Abs(ratio - (float)2560/1600) < e) targetMatch = 0.45f;
      else if (Mathf.Abs(ratio - (float)1600/2560) < e) targetMatch = 0.45f;
      if (matchTween != null && matchTween.IsActive()) matchTween.Kill();
      matchTween = DOTween.To(() => CanvasScaler.matchWidthOrHeight, x => CanvasScaler.matchWidthOrHeight = x, targetMatch, transitionDuration).SetEase(Ease.InOutQuad);

      Debug.Log($"matchWidthOrHeight set to: {targetMatch}");
    }
    else
    {
      Debug.LogWarning("Unity: Invalid format received in SwitchDisplay");
    }
  }


#if UNITY_EDITOR
  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      SwitchDisplay(Screen.width + "," + Screen.height);  
    }
  }
#endif
}
