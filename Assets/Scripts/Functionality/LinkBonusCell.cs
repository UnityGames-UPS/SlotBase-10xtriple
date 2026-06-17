using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LinkBonusCell : MonoBehaviour
{
  [SerializeField] private RectTransform SpinStrip;
  [SerializeField] private Image[] StripSymbols;
  [SerializeField] public ImageAnimation CellAnim;
  [SerializeField] private Image BoxBackground;
  [SerializeField] private GameObject PinkGlow;
  [SerializeField] private GameObject ZoneBackground;
  [SerializeField] private TMP_Text MultiplierText;
  [SerializeField] private TMP_Text PrizeValueText;
  [SerializeField] private float cellHeight = 160f;
  [SerializeField] private float spinSpeed = 0.15f;
  [SerializeField] private float flickerFadeDuration = 0.08f;

  private bool _isLocked = false;
  private Tween _spinTween;
  private Image _zoneBackgroundImage;

  public bool IsLocked => _isLocked;
  public RectTransform CellRect => (RectTransform)transform;
  public Image FirstStripSymbol => StripSymbols != null && StripSymbols.Length > 0 ? StripSymbols[0] : null;

  public void Initialize(Sprite defaultSprite)
  {
    _isLocked = false;
    _spinTween?.Kill();
    if (SpinStrip) SpinStrip.anchoredPosition = Vector2.zero;
    foreach (var sym in StripSymbols)
      if (sym) { sym.sprite = defaultSprite; sym.color = Color.white; }
    if (BoxBackground) { var c = BoxBackground.color; c.a = 1f; BoxBackground.color = c; }
    if (PinkGlow) PinkGlow.SetActive(false);
    if (ZoneBackground)
    {
      _zoneBackgroundImage = ZoneBackground.GetComponent<Image>();
      ZoneBackground.SetActive(true);
      if (_zoneBackgroundImage) _zoneBackgroundImage.color = new Color(1, 1, 1, 0);
    }
    if (MultiplierText) MultiplierText.gameObject.SetActive(false);
    if (PrizeValueText) PrizeValueText.gameObject.SetActive(false);
  }

  public void StartSpin()
  {
    if (_isLocked) return;
    _spinTween?.Kill();
    if (SpinStrip) SpinStrip.anchoredPosition = Vector2.zero;
    _spinTween = SpinStrip.DOAnchorPosY(-cellHeight, spinSpeed)
      .SetEase(Ease.Linear)
      .SetLoops(-1, LoopType.Restart);
  }

  public void StopAt(Sprite resultSprite, double? prizeValue, bool playAnimation = false)
  {
    if (_isLocked) return;
    _spinTween?.Kill();
    if (SpinStrip) SpinStrip.anchoredPosition = Vector2.zero;
    if (StripSymbols != null && StripSymbols.Length > 0 && resultSprite != null)
      StripSymbols[0].sprite = resultSprite;
    if (prizeValue.HasValue && prizeValue.Value > 0 && PrizeValueText)
    {
      PrizeValueText.text = prizeValue.Value.ToString("F2");
      PrizeValueText.gameObject.SetActive(true);
    }
    if (playAnimation && CellAnim) CellAnim.StartAnimation();
  }

  public void Freeze()
  {
    _isLocked = true;
    _spinTween?.Kill();
    if (SpinStrip) SpinStrip.anchoredPosition = Vector2.zero;
  }

  public void ShowZoneMultiplier(int multiplier)
  {
    if (_zoneBackgroundImage) _zoneBackgroundImage.DOFade(1f, 0.2f);
    if (MultiplierText) { MultiplierText.text = $"{multiplier}x"; MultiplierText.gameObject.SetActive(true); }
  }

  public void ShowPreSpinGlow()
  {
    if (_isLocked) return;
    if (PinkGlow) PinkGlow.SetActive(true);
  }

  public void HidePreSpinGlow()
  {
    if (PinkGlow) PinkGlow.SetActive(false);
  }

  public void ShowFlickerZone()
  {
    if (_zoneBackgroundImage) _zoneBackgroundImage.DOFade(1f, flickerFadeDuration);
  }

  public void HideFlickerZone()
  {
    if (_isLocked) return;
    if (_zoneBackgroundImage) _zoneBackgroundImage.DOFade(0f, flickerFadeDuration);
  }

  public void TriggerFlashPinkGlow() => StartCoroutine(FlashPinkGlow());

  private IEnumerator FlashPinkGlow()
  {
    if (_isLocked || PinkGlow == null) yield break;
    PinkGlow.SetActive(true);
    yield return new WaitForSeconds(0.2f);
    PinkGlow.SetActive(false);
  }

  public void DimCell()
  {
    if (StripSymbols != null)
      foreach (var s in StripSymbols)
        if (s) s.DOFade(0.4f, 0.3f);
    if (BoxBackground) BoxBackground.DOFade(0.4f, 0.3f);
  }

  public void ResetCell()
  {
    _isLocked = false;
    _spinTween?.Kill();
    if (SpinStrip) SpinStrip.anchoredPosition = Vector2.zero;
    if (PinkGlow) PinkGlow.SetActive(false);
    if (_zoneBackgroundImage) _zoneBackgroundImage.color = new Color(1, 1, 1, 0);
    if (MultiplierText) MultiplierText.gameObject.SetActive(false);
    if (PrizeValueText) PrizeValueText.gameObject.SetActive(false);
    if (StripSymbols != null)
      foreach (var s in StripSymbols)
        if (s) s.color = Color.white;
    if (BoxBackground) { var c = BoxBackground.color; c.a = 1f; BoxBackground.color = c; }
    if (CellAnim) CellAnim.StopAnimation();
  }
}
