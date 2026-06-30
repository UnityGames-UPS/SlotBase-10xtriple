using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
  internal event Action<bool> OnInfoScreenToggled;

  [Header("Menu UI")]
  [SerializeField]
  private Button Menu_Button;
  [SerializeField]
  private GameObject Menu_Object;

  [Header("Settings UI")]
  [SerializeField]
  private Button Settings_Button;
  [SerializeField]
  private GameObject Settings_Object;
  [SerializeField]
  private RectTransform Settings_RT;

  [SerializeField]
  private Button Exit_Button;
  [SerializeField]
  private GameObject Exit_Object;

  [Header("Betting UI")]
  [SerializeField] private TMP_Text TotalBetAmountText;
  [SerializeField] private TMP_Text Balance_text;
  [SerializeField] private TMP_Text TotalBet_text;
  [SerializeField] private TMP_Text TotalWin_text;

  [Header("Intro")]
  [SerializeField] private RectTransform GameContent;

  [Header("Free Spin Trigger Sequence")]
  [SerializeField] private GameObject DarkenOverlay;
  [SerializeField] private GameObject FreeSpinsTriggerText;
  [SerializeField] private GameObject MainLogo;
  [SerializeField] private GameObject FreeSpinsLogoDisplay;
  [SerializeField] private TMP_Text FreeSpinsLogoCountText;
  [SerializeField] private TMP_Text FreeSpinsAwardedText;
  [SerializeField] private float freeSpinsPopupScaleDuration = 0.4f;
  [SerializeField] private float freeSpinsPopupHoldDuration = 1.5f;

  [Header("Bonus Win Sequence")]
  [SerializeField] private GameObject BonusWinSequencePanel;
  [SerializeField] private ImageAnimation BonusWinCoinFallingAnim;
  [SerializeField] private RectTransform BonusWinPanel;
  [SerializeField] private Image BonusNameGraphicImage;
  [SerializeField] private TMP_Text BonusWinAmountText;
  [SerializeField] private Sprite BigWinTierSprite;
  [SerializeField] private Sprite MegaWinTierSprite;
  [SerializeField] private Sprite SuperWinTierSprite;
  [SerializeField] private float bonusWinShowDelay = 1f;
  [SerializeField] private float bonusWinScaleDuration = 0.4f;
  [SerializeField] private float bonusWinCountDuration = 1.5f;
  [SerializeField] private float bonusWinHoldDuration = 2f;

  private const double BigWinThreshold = 3;
  private const double MegaWinThreshold = 6;
  private const double SuperWinThreshold = 10;

  [Header("Ticker UI")]
  [SerializeField] private RectTransform TickerContainer;
  [SerializeField] private TMP_Text TickerText;
  [SerializeField] private float TickerDuration = 3f;

  private readonly string[] tickerMessages = { "GOOD LUCK", "ALL THE BEST" };
  private int tickerIndex = 0;
  private Tween tickerTween;

  private List<double> betAmounts;
  internal int BetCount => betAmounts?.Count ?? 0;
  internal double GetBetAmount(int index) => betAmounts[index];

  [Header("Information UI")]
  [SerializeField]
  private Button Info_Button;
  [SerializeField]
  private GameObject InfoSlidesPanel;
  [SerializeField]
  private Button BackToGame_Button;
  [SerializeField]
  private Button NextButton;
  [SerializeField]
  private Button PrevButton;
  [SerializeField]
  private Image SlideContainer;
  [SerializeField]
  private Sprite[] InfoSlides;

  private int currentSlideIndex = 0;

  [Header("Popus UI")]
  [SerializeField]
  private GameObject PopupsPanel;

  [Header("About Popup")]
  [SerializeField]
  private GameObject AboutPopup_Object;
  [SerializeField]
  private Button AboutExit_Button;

  [Header("Settings Popup")]
  [SerializeField]
  private GameObject SettingsPopup_Object;
  [SerializeField]
  private Button SettingsExit_Button;
  [SerializeField]
  private Button Sound_Button;
  [SerializeField]
  private Button Music_Button;

  [SerializeField]
  private Sprite MusicOnSprite;
  [SerializeField]
  private Sprite MusicOffSprite;
  [SerializeField]
  private Sprite SoundOnSprite;
  [SerializeField]
  private Sprite SoundOffSprite;

  [Header("Spin Win Display")]
  [SerializeField] private GameObject SpinWinPanel;
  [SerializeField] private TMP_Text SpinWinText;
  [SerializeField] private GameObject SpinWinCoinSplash;
  [SerializeField] private ImageAnimation SpinWinCoinSplashAnim;
  [SerializeField] private float spinWinCountDuration = 1f;

  [Header("Disconnection Popup")]
  [SerializeField]
  private Button CloseDisconnect_Button;
  [SerializeField]
  private GameObject DisconnectPopup_Object;

  [Header("AnotherDevice Popup")]
  [SerializeField]
  private Button CloseAD_Button;
  [SerializeField]
  private GameObject ADPopup_Object;

  [Header("Reconnection Popup")]
  [SerializeField]
  private TMP_Text ReconnectingText;
  [SerializeField]
  private TMP_Text ReconnectingAttemptText;
  [SerializeField]
  private GameObject ReconnectPopup_Object;

  [Header("LowBalance Popup")]
  [SerializeField]
  private Button LBExit_Button;
  [SerializeField]
  private GameObject LBPopup_Object;

  [Header("Quit Popup")]
  [SerializeField]
  private GameObject QuitPopup_Object;
  [SerializeField]
  private Button YesQuit_Button;
  [SerializeField]
  private Button NoQuit_Button;
  [SerializeField]
  private Button CrossQuit_Button;

  [Header("References")]
  [SerializeField] private AudioManager audioManager;
  [SerializeField] private Button GameExit_Button;
  [SerializeField] private Button Home_Button;

  [SerializeField]
  private SlotBehaviour slotManager;

  private Tween _balanceTween;
  private bool isMusic = true;
  private bool isSound = true;
  internal bool isExit = false;

  private void Start()
  {
    StartCoroutine(PlayIntro());

    if (Menu_Button) Menu_Button.onClick.RemoveAllListeners();
    if (Menu_Button) Menu_Button.onClick.AddListener(OpenMenu);

    if (Exit_Button) Exit_Button.onClick.RemoveAllListeners();
    if (Exit_Button) Exit_Button.onClick.AddListener(CloseMenu);

    if (AboutExit_Button) AboutExit_Button.onClick.RemoveAllListeners();
    if (AboutExit_Button) AboutExit_Button.onClick.AddListener(delegate { ClosePopup(AboutPopup_Object); });

    if (InfoSlidesPanel) InfoSlidesPanel.SetActive(false);

    if (Info_Button) Info_Button.onClick.RemoveAllListeners();
    if (Info_Button) Info_Button.onClick.AddListener(() =>
    {
      if (audioManager) audioManager.PlayUIClick();
      currentSlideIndex = 0;
      InfoSlidesPanel.SetActive(true);
      ShowSlide(currentSlideIndex);
      OnInfoScreenToggled?.Invoke(true);

      if (BackToGame_Button) BackToGame_Button.onClick.RemoveAllListeners();
      if (BackToGame_Button) BackToGame_Button.onClick.AddListener(() =>
      {
        InfoSlidesPanel.SetActive(false);
        OnInfoScreenToggled?.Invoke(false);
      });

      if (NextButton) NextButton.onClick.RemoveAllListeners();
      if (NextButton) NextButton.onClick.AddListener(() =>
      {
        currentSlideIndex = (currentSlideIndex + 1) % InfoSlides.Length;
        ShowSlide(currentSlideIndex);
      });

      if (PrevButton) PrevButton.onClick.RemoveAllListeners();
      if (PrevButton) PrevButton.onClick.AddListener(() =>
      {
        currentSlideIndex = (currentSlideIndex - 1 + InfoSlides.Length) % InfoSlides.Length;
        ShowSlide(currentSlideIndex);
      });
    });

    if (Settings_Button) Settings_Button.onClick.RemoveAllListeners();
    if (Settings_Button) Settings_Button.onClick.AddListener(delegate { OpenPopup(SettingsPopup_Object); });

    if (SettingsExit_Button) SettingsExit_Button.onClick.RemoveAllListeners();
    if (SettingsExit_Button) SettingsExit_Button.onClick.AddListener(delegate { ClosePopup(SettingsPopup_Object); });

    SetButtonSprite(Music_Button, isMusic ? MusicOnSprite : MusicOffSprite);
    SetButtonSprite(Sound_Button, isSound ? SoundOnSprite : SoundOffSprite);

    if (GameExit_Button) GameExit_Button.onClick.RemoveAllListeners();
    if (GameExit_Button) GameExit_Button.onClick.AddListener(delegate
    {
      OpenPopup(QuitPopup_Object);
    });

    if (Home_Button) Home_Button.onClick.RemoveAllListeners();
    if (Home_Button) Home_Button.onClick.AddListener(CallOnExitFunction);

    if (NoQuit_Button) NoQuit_Button.onClick.RemoveAllListeners();
    if (NoQuit_Button) NoQuit_Button.onClick.AddListener(delegate
    {
      if (!isExit)
      {
        ClosePopup(QuitPopup_Object);
      }
    });

    if (CrossQuit_Button) CrossQuit_Button.onClick.RemoveAllListeners();
    if (CrossQuit_Button) CrossQuit_Button.onClick.AddListener(delegate
    {
      if (!isExit)
      {
        ClosePopup(QuitPopup_Object);
      }
    });

    if (LBExit_Button) LBExit_Button.onClick.RemoveAllListeners();
    if (LBExit_Button) LBExit_Button.onClick.AddListener(delegate { ClosePopup(LBPopup_Object); });

    if (YesQuit_Button) YesQuit_Button.onClick.RemoveAllListeners();
    if (YesQuit_Button) YesQuit_Button.onClick.AddListener(delegate
    {
      CallOnExitFunction();
      Debug.Log("quit event: pressed YES Button ");

    });

    if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.RemoveAllListeners();
    if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.AddListener(CallOnExitFunction); //BackendChanges

    if (CloseAD_Button) CloseAD_Button.onClick.RemoveAllListeners();
    if (CloseAD_Button) CloseAD_Button.onClick.AddListener(CallOnExitFunction);

    isMusic = audioManager == null || audioManager.MusicEnabled;
    isSound = audioManager == null || audioManager.SfxEnabled;

    if (Sound_Button) Sound_Button.onClick.RemoveAllListeners();
    if (Sound_Button) Sound_Button.onClick.AddListener(ToggleSound);

    if (Music_Button) Music_Button.onClick.RemoveAllListeners();
    if (Music_Button) Music_Button.onClick.AddListener(ToggleMusic);

    if (FreeSpinsLogoDisplay) FreeSpinsLogoDisplay.SetActive(false);
  }

  private IEnumerator PlayIntro()
  {
    if (GameContent)
    {
      Vector3 fullScale = GameContent.localScale;
      GameContent.localScale = new Vector3(0.8f, 0.8f, 0.8f);
      yield return DOTween.Sequence()
        .Append(GameContent.DOScale(fullScale, 0.5f).SetEase(Ease.OutCubic))
        .Append(GameContent.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.35f).SetEase(Ease.InOutCubic))
        .Append(GameContent.DOScale(fullScale, 0.45f).SetEase(Ease.OutCubic))
        .WaitForCompletion();
    }
  }

  internal void LowBalPopup()
  {
    OpenPopup(LBPopup_Object);
  }

  internal void DisconnectionPopup()
  {
    if (!isExit)
    {
      OpenPopup(DisconnectPopup_Object);
    }
  }

  internal void ReconnectionPopup(int attempt, int max)
  {
    if (ReconnectingText) ReconnectingText.text = "Reconnecting...";
    if (ReconnectingAttemptText) ReconnectingAttemptText.text = $"{attempt}/{max}";
    OpenPopup(ReconnectPopup_Object);
  }

  internal void CheckAndClosePopups()
  {
    if (ReconnectPopup_Object != null && ReconnectPopup_Object.activeInHierarchy)
    {
      ClosePopup(ReconnectPopup_Object);
    }
    if (DisconnectPopup_Object != null && DisconnectPopup_Object.activeInHierarchy)
    {
      ClosePopup(DisconnectPopup_Object);
    }
  }

  internal void ADfunction()
  {
    OpenPopup(ADPopup_Object);
  }

  private void CallOnExitFunction()
  {
    if (!isExit)
    {
      isExit = true;
      if (audioManager) audioManager.PlayButton();
      slotManager.CallCloseSocket();
    }
  }

  private void OpenMenu()
  {
    audioManager.PlayButton();
    if (Menu_Object) Menu_Object.SetActive(false);
    if (Exit_Object) Exit_Object.SetActive(true);
    if (Settings_Object) Settings_Object.SetActive(true);

    DOTween.To(() => Settings_RT.anchoredPosition, (val) => Settings_RT.anchoredPosition = val, new Vector2(Settings_RT.anchoredPosition.x, Settings_RT.anchoredPosition.y + 250), 0.1f).OnUpdate(() =>
    {
      LayoutRebuilder.ForceRebuildLayoutImmediate(Settings_RT);
    });
  }

  private void CloseMenu()
  {

    if (audioManager) audioManager.PlayButton();
    DOTween.To(() => Settings_RT.anchoredPosition, (val) => Settings_RT.anchoredPosition = val, new Vector2(Settings_RT.anchoredPosition.x, Settings_RT.anchoredPosition.y - 250), 0.1f).OnUpdate(() =>
    {
      LayoutRebuilder.ForceRebuildLayoutImmediate(Settings_RT);
    });

    DOVirtual.DelayedCall(0.1f, () =>
     {
       if (Menu_Object) Menu_Object.SetActive(true);
       if (Exit_Object) Exit_Object.SetActive(false);
       if (Settings_Object) Settings_Object.SetActive(false);
     });
  }

  private void OpenPopup(GameObject Popup)
  {
    if (audioManager) audioManager.PlayButton();
    if (Popup) Popup.SetActive(true);
    if (PopupsPanel) PopupsPanel.SetActive(true);
  }

  internal void ClosePopup(GameObject Popup)
  {
    if (audioManager) audioManager.PlayButton();
    if (Popup) Popup.SetActive(false);
    if (DisconnectPopup_Object == null || !DisconnectPopup_Object.activeSelf)
    {
      if (PopupsPanel) PopupsPanel.SetActive(false);
    }
  }

  private void ToggleMusic()
  {
    isMusic = !isMusic;
    SetButtonSprite(Music_Button, isMusic ? MusicOnSprite : MusicOffSprite);
    if (audioManager) audioManager.PlayUIClick();
    if (audioManager) audioManager.SetMusicEnabled(isMusic);
  }

  private void ToggleSound()
  {
    isSound = !isSound;
    SetButtonSprite(Sound_Button, isSound ? SoundOnSprite : SoundOffSprite);
    if (audioManager) audioManager.PlayUIClick();
    if (audioManager) audioManager.SetSfxEnabled(isSound);
  }

  private void SetButtonSprite(Button button, Sprite sprite)
  {
    if (button == null || sprite == null) return;
    var img = button.GetComponent<Image>();
    if (img) img.sprite = sprite;
  }

  internal void InitialiseBalanceAndWin(double balance, double bet)
  {
    if (Balance_text) Balance_text.text = balance.ToString("F3");
    if (TotalBet_text) TotalBet_text.text = bet.ToString();
    if (TotalWin_text) TotalWin_text.text = "0.000";
  }

  internal IEnumerator PlayFreeSpinTriggerSequence(int spinCount)
  {
    if (DarkenOverlay) DarkenOverlay.SetActive(true);
    if (FreeSpinsTriggerText) FreeSpinsTriggerText.SetActive(true);

    if (FreeSpinsAwardedText)
    {
      FreeSpinsAwardedText.text = spinCount.ToString();
      FreeSpinsAwardedText.gameObject.SetActive(true);
      FreeSpinsAwardedText.transform.localScale = Vector3.zero;
      yield return FreeSpinsAwardedText.transform.DOScale(Vector3.one, freeSpinsPopupScaleDuration).SetEase(Ease.OutBack).WaitForCompletion();
    }

    yield return new WaitForSeconds(freeSpinsPopupHoldDuration);

    if (FreeSpinsAwardedText) FreeSpinsAwardedText.gameObject.SetActive(false);
    if (MainLogo) MainLogo.SetActive(false);
    if (FreeSpinsLogoCountText) FreeSpinsLogoCountText.text = spinCount.ToString();
    if (FreeSpinsLogoDisplay) FreeSpinsLogoDisplay.SetActive(true);
    if (DarkenOverlay) DarkenOverlay.SetActive(false);
    if (FreeSpinsTriggerText) FreeSpinsTriggerText.SetActive(false);
  }

  internal void EndFreeSpinTriggerSequence()
  {
    if (MainLogo) MainLogo.SetActive(true);
    if (FreeSpinsLogoDisplay) FreeSpinsLogoDisplay.SetActive(false);
  }

  internal void UpdateFreeSpinsRemaining(int remaining)
  {
    if (FreeSpinsLogoCountText) FreeSpinsLogoCountText.text = remaining.ToString();
  }

  internal IEnumerator ShowSpinWin(double winAmount)
  {
    if (winAmount <= 0) yield break;
    if (SpinWinPanel)
    {
      SpinWinPanel.SetActive(true);
      SpinWinPanel.transform.localScale = Vector3.one;
      SpinWinPanel.transform.DOScale(1.2f, spinWinCountDuration).SetEase(Ease.OutQuad);
    }
    if (SpinWinCoinSplash) SpinWinCoinSplash.SetActive(true);
    if (SpinWinCoinSplashAnim)
    {
      SpinWinCoinSplashAnim.doLoopAnimation = false;
      SpinWinCoinSplashAnim.StartAnimation();
    }
    float display = 0f;
    if (TotalWin_text) TotalWin_text.text = "0.000";
    if (TotalWin_text) DOTween.To(() => display, v => { TotalWin_text.text = v.ToString("F3"); }, (float)winAmount, spinWinCountDuration);
    if (SpinWinText)
      yield return DOTween.To(() => display, v => { display = v; SpinWinText.text = v.ToString("F3"); },
        (float)winAmount, spinWinCountDuration).WaitForCompletion();
    yield return new WaitForSeconds(0.5f);
    HideSpinWin();
  }

  internal void HideSpinWin()
  {
    if (SpinWinPanel) SpinWinPanel.SetActive(false);
    if (SpinWinCoinSplash) SpinWinCoinSplash.SetActive(false);
    if (SpinWinText) SpinWinText.text = "";
  }

  internal void ResetTotalWin()
  {
    if (TotalWin_text) TotalWin_text.text = "0.000";
  }

  internal void UpdateTotalWin(double amount)
  {
    if (TotalWin_text) TotalWin_text.text = amount.ToString("F3");
  }

  internal void UpdateBalance(double amount)
  {
    _balanceTween?.Kill();
    if (Balance_text) Balance_text.text = amount.ToString("F3");
  }

  internal void AnimateBalanceDeduction(double from, double to)
  {
    _balanceTween?.Kill();
    double current = from;
    _balanceTween = DOTween.To(() => current, v => { current = v; if (Balance_text) Balance_text.text = current.ToString("F3"); }, to, 0.8f);
  }

  internal void InitialiseUI(List<double> bets, List<Symbol> symbols)
  {
    betAmounts = bets;
    UpdateBetDisplay(betAmounts[0]);
  }

  internal void SetBet(int betIndex)
  {
    UpdateBetDisplay(betAmounts[betIndex]);
  }

  internal void ShowTicker()
  {
    if (TickerContainer == null || TickerText == null) return;

    tickerTween?.Kill();

    TickerText.text = tickerMessages[tickerIndex];
    tickerIndex = (tickerIndex + 1) % tickerMessages.Length;

    float containerWidth = TickerContainer.rect.width;
    float startX = containerWidth / 2f + TickerText.preferredWidth / 2f;
    float endX = -(containerWidth / 2f + TickerText.preferredWidth / 2f);

    TickerText.rectTransform.anchoredPosition = new Vector2(startX, TickerText.rectTransform.anchoredPosition.y);
    tickerTween = TickerText.rectTransform.DOAnchorPosX(endX, TickerDuration).SetEase(Ease.Linear);
  }

  private Sprite GetBonusWinTierSprite(string tier)
  {
    switch (tier)
    {
      case "big": return BigWinTierSprite;
      case "mega": return MegaWinTierSprite;
      case "super": return SuperWinTierSprite;
      default: return null;
    }
  }

  private static string GetBonusWinTier(double totalWin, double bet)
  {
    if (bet <= 0) return null;
    double ratio = totalWin / bet;
    if (ratio >= SuperWinThreshold) return "super";
    if (ratio >= MegaWinThreshold) return "mega";
    if (ratio >= BigWinThreshold) return "big";
    return null;
  }

  internal IEnumerator ShowBonusWinSequence(double totalWin, double bet)
  {
    string tier = GetBonusWinTier(totalWin, bet);
    if (tier == null) yield break;

    yield return new WaitForSeconds(bonusWinShowDelay);

    if (BonusNameGraphicImage) BonusNameGraphicImage.sprite = GetBonusWinTierSprite(tier);
    if (BonusWinPanel) BonusWinPanel.localScale = Vector3.zero;
    if (BonusWinAmountText) BonusWinAmountText.text = "0.000";
    if (BonusWinSequencePanel) BonusWinSequencePanel.SetActive(true);

    if (audioManager) audioManager.PlayBigWin();
    if (BonusWinCoinFallingAnim) { BonusWinCoinFallingAnim.doLoopAnimation = true; BonusWinCoinFallingAnim.StopAnimation(); BonusWinCoinFallingAnim.StartAnimation(); }

    if (BonusWinPanel) BonusWinPanel.DOScale(Vector3.one, bonusWinScaleDuration).SetEase(Ease.OutBack);
    yield return new WaitForSeconds(bonusWinScaleDuration);

    float bonusWinDisplay = 0f;
    if (BonusWinAmountText)
      yield return DOTween.To(() => bonusWinDisplay, v => { bonusWinDisplay = v; BonusWinAmountText.text = v.ToString("F3"); },
        (float)totalWin, bonusWinCountDuration).WaitForCompletion();
    else
      yield return new WaitForSeconds(bonusWinCountDuration);

    yield return new WaitForSeconds(bonusWinHoldDuration);

    if (BonusWinCoinFallingAnim) { BonusWinCoinFallingAnim.StopAnimation(); BonusWinCoinFallingAnim.doLoopAnimation = false; }
    if (BonusWinSequencePanel) BonusWinSequencePanel.SetActive(false);
  }

  private void UpdateBetDisplay(double bet)
  {
    if (TotalBetAmountText) TotalBetAmountText.text = bet.ToString("F2");
  }

  private void ShowSlide(int index)
  {
    if (SlideContainer && InfoSlides != null && InfoSlides.Length > 0)
      SlideContainer.sprite = InfoSlides[index];
  }

}
