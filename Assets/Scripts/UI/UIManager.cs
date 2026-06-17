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
  [SerializeField] private TMP_Text MiniPayoutText;
  [SerializeField] private TMP_Text MinorPayoutText;
  [SerializeField] private TMP_Text MajorPayoutText;
  [SerializeField] private TMP_Text MegaPayoutText;
  [SerializeField] private TMP_Text GrandPayoutText;

  [Header("Pinata Meters UI")]
  [SerializeField] private TMP_Text GreenMeterText;
  [SerializeField] private TMP_Text RedMeterText;
  [SerializeField] private TMP_Text BlueMeterText;

  [Header("Reel Frame")]
  [SerializeField] private Image ReelFrame;
  [SerializeField] private Sprite DefaultReelFrameSprite;
  [SerializeField] private Sprite RedReelFrameSprite;

  [Header("Intro")]
  [SerializeField] private RectTransform PinataTrio;
  [SerializeField] private ImageAnimation PinataTrioAnim;
  [SerializeField] private RectTransform GameContent;
  [SerializeField] private RectTransform MoveUpAfterIntro;
  [SerializeField] private float introHoldDuration = 3f;
  [SerializeField] private float offscreenOffset = 1200f;
  [SerializeField] private float gameContentScrollAmount = 700f;
  [SerializeField] private float gameContentRestY = -0.66f;
  [SerializeField] private RectTransform GreenPinata;
  [SerializeField] private RectTransform RedPinata;
  [SerializeField] private RectTransform BluePinata;
  [SerializeField] private float pinataScrollAmount = 250f;
  [SerializeField] private float pinataRedDelay = 0.3f;

  [Header("Pinata Stage Animations")]
  [SerializeField] private Sprite[] GreenPinataStage1Sprites;
  [SerializeField] private Sprite[] GreenPinataStage2Sprites;
  [SerializeField] private Sprite[] RedPinataStage1Sprites;
  [SerializeField] private Sprite[] RedPinataStage2Sprites;
  [SerializeField] private Sprite[] BluePinataStage1Sprites;
  [SerializeField] private Sprite[] BluePinataStage2Sprites;
  [SerializeField] private Sprite[] BustedRedPinataAnimSprites;
  [SerializeField] private Sprite[] BustedBluePinataAnimSprites;

  [Header("Feature Intro")]
  [SerializeField] private GameObject GreenPinataIntroObject;
  [SerializeField] private ImageAnimation GreenPinataIntroAnim;
  [SerializeField] private Image WheelBonusNameGraphic;
  [SerializeField] private Image WheelBonusGlowGraphic;
  [SerializeField] private GameObject RedPinataIntroObject;
  [SerializeField] private ImageAnimation RedPinataIntroAnim;
  [SerializeField] private Image PickJackpotNameGraphic;
  [SerializeField] private Image PickJackpotGlowGraphic;
  [SerializeField] private GameObject BluePinataIntroObject;
  [SerializeField] private ImageAnimation BluePinataIntroAnim;
  [SerializeField] private Image LinkBonusNameGraphic;
  [SerializeField] private Image LinkBonusGlowGraphic;
  [SerializeField] private float featureNameScaleDuration = 0.4f;
  [SerializeField] private float featureNameHoldDuration = 1f;
  [SerializeField] private float featureNameExpandDuration = 1.5f;
  [SerializeField] private float featureNameFadeDuration = 0.5f;
  [SerializeField] private float introShakeStartOffset = 2.5f;
  [SerializeField] private float introScaleAmount = 0.93f;
  [SerializeField] private float introScaleDownDuration = 0.25f;
  [SerializeField] private float introShakeDuration = 1.8f;
  [SerializeField] private float introShakeStrength = 0.015f;
  [SerializeField] private int introShakeVibrato = 20;
  [SerializeField] private float introScaleUpDuration = 0.35f;

  [Header("Feature Pinata UI")]
  [SerializeField] private RectTransform Payouts;
  [SerializeField] private float payoutsExtraSlide = 250f;
  [SerializeField] private float payoutsSlideDownDuration = 0.3f;
  [SerializeField] private float payoutsSlideUpDuration = 0.5f;
  [SerializeField] private float featureHideAmount = 550f;
  [SerializeField] private float slideContentDuration = 0.8f;
  [SerializeField] private GameObject BustedRedPinata;
  [SerializeField] private Sprite BustedBluePinataSprite;
  [SerializeField] private Image SmallReelFrame;
  [SerializeField] private Sprite GoalFrameSprite;
  [SerializeField] private Sprite SpinsRemainingFrameSprite;
  [SerializeField] private float pinataEarlyStart = 0.15f;

  [Header("Link Bonus UI")]
  [SerializeField] private GameObject LinkBonusFeature;
  [SerializeField] private TMP_Text LinkBonusSpinsRemainingText;
  [SerializeField] private GameObject SlotMain;
  [SerializeField] private GameObject AnimationLayer;

  [Header("Wheel Bonus UI")]
  [SerializeField] private TMP_Text WheelBonusGrandText;
  [SerializeField] private TMP_Text WheelBonusMegaText;
  [SerializeField] private TMP_Text WheelBonusMajorText;
  [SerializeField] private TMP_Text WheelBonusMinorText;
  [SerializeField] private TMP_Text WheelBonusMiniText;

  [Header("Pick Jackpot UI")]
  [SerializeField] private GameObject PickJackpotPanel;
  [SerializeField] private Button[] PinataButtons;
  [SerializeField] private Image[] JackpotRevealImages;
  [SerializeField] private TMP_Text PickJackpotTimerText;
  [SerializeField] private RectTransform FallingJackpotRT;
  [SerializeField] private Image FallingJackpotImage;
  [SerializeField] private Transform JackpotLandingPoint;
  [SerializeField] private Sprite MiniJackpotSprite;
  [SerializeField] private Sprite MinorJackpotSprite;
  [SerializeField] private Sprite MajorJackpotSprite;
  [SerializeField] private Sprite MegaJackpotSprite;
  [SerializeField] private Sprite GrandJackpotSprite;
  [SerializeField] private float pickJackpotTimerDuration = 10f;
  [SerializeField] private ImageAnimation TimerClockAnimation;
  [SerializeField] private ImageAnimation[] PinataButtonAnimations;
  [SerializeField] private Image FreeSpinsUntilImage;
  [SerializeField] private float jackpotLaunchHeight = 500f;
  [SerializeField] private CanvasGroup PickingJackpotGroup;
  [SerializeField] private GameObject JackpotPickedObject;
  [SerializeField] private CanvasGroup JackpotPickedGroup;
  [SerializeField] private float jackpotTextFadeDuration = 0.4f;
  [SerializeField] private Vector2 jackpotCenterTarget = new Vector2(0f, -90f);

  [Header("Jackpot Win Sequence")]
  [SerializeField] private ImageAnimation CashFallingAnim;
  [SerializeField] private ImageAnimation CoinFallingAnim;
  [SerializeField] private RectTransform JackpotWinGraphic;
  [SerializeField] private Image JackpotWinGraphicImage;
  [SerializeField] private Sprite MiniWinSprite;
  [SerializeField] private Sprite MinorWinSprite;
  [SerializeField] private Sprite MajorWinSprite;
  [SerializeField] private Sprite MegaWinSprite;
  [SerializeField] private Sprite GrandWinSprite;
  [SerializeField] private GameObject JackpotWinSequencePanel;
  [SerializeField] private GameObject JackpotAmountPanel;
  [SerializeField] private TMP_Text JackpotWinAmountText;
  [SerializeField] private float jackpotGraphicDropDuration = 0.6f;
  [SerializeField] private float jackpotPanelExpandDuration = 0.4f;
  [SerializeField] private float jackpotCountDuration = 1.5f;
  [SerializeField] private float jackpotHoldDuration = 2f;
  [SerializeField] private float coinsLingerDuration = 1f;

  internal bool PickJackpotSelected = false;
  private int _selectedPinataIndex = -1;
  private Coroutine _pickJackpotTimerRoutine;
  private readonly string[] _jackpotOrder = { "mini", "minor", "major", "mega", "grand" };

  [Header("Bonus Win Sequence")]
  [SerializeField] private GameObject BonusWinSequencePanel;
  [SerializeField] private ImageAnimation BonusWinCoinFallingAnim;
  [SerializeField] private RectTransform BonusWinPanel;
  [SerializeField] private Image BonusNameGraphicImage;
  [SerializeField] private TMP_Text BonusWinAmountText;
  [SerializeField] private Sprite BigWinTierSprite;
  [SerializeField] private Sprite MegaWinTierSprite;
  [SerializeField] private Sprite SuperWinTierSprite;
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
  private double[] jackpotMultipliers = new double[5];
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

  private CanvasGroup _payoutsCanvasGroup;
  private Tween _balanceTween;
  private bool isMusic = true;
  private bool isSound = true;
  internal bool isExit = false;
  private Vector2 _pickJackpotPanelOrigin;
  private Vector2[] _jackpotRevealImageOrigins;
  private Vector2 _greenPinataOrigin;
  private Vector2 _redPinataOrigin;
  private Vector2 _bluePinataOrigin;
  private Vector2 _smallReelFrameOrigin;
  private Sprite _redPinataOriginalSprite;
  private Sprite _bluePinataOriginalSprite;
  private ImageAnimation _greenPinataAnim;
  private ImageAnimation _redPinataAnim;
  private ImageAnimation _bluePinataAnim;
  private List<Sprite> _greenPinataBaseSprites;
  private List<Sprite> _redPinataBaseSprites;
  private List<Sprite> _bluePinataBaseSprites;
  private int _greenPinataStage = 0;
  private int _redPinataStage = 0;
  private int _bluePinataStage = 0;
  private int _greenMeterThreshold = 0;
  private int _redMeterThreshold = 0;
  private int _blueMeterThreshold = 0;

  private void Start()
  {
    if (PickJackpotPanel) _pickJackpotPanelOrigin = PickJackpotPanel.GetComponent<RectTransform>().anchoredPosition;
    if (JackpotRevealImages != null)
    {
      _jackpotRevealImageOrigins = new Vector2[JackpotRevealImages.Length];
      for (int i = 0; i < JackpotRevealImages.Length; i++)
        _jackpotRevealImageOrigins[i] = JackpotRevealImages[i].rectTransform.anchoredPosition;
    }
    if (Payouts) _payoutsCanvasGroup = Payouts.GetComponent<CanvasGroup>();
    if (GreenPinata)
    {
      _greenPinataOrigin = GreenPinata.anchoredPosition;
      _greenPinataAnim = GreenPinata.GetComponent<ImageAnimation>();
      if (_greenPinataAnim) _greenPinataBaseSprites = new List<Sprite>(_greenPinataAnim.textureArray);
    }
    if (RedPinata)
    {
      _redPinataOrigin = RedPinata.anchoredPosition;
      _redPinataOriginalSprite = RedPinata.GetComponent<Image>()?.sprite;
      _redPinataAnim = RedPinata.GetComponent<ImageAnimation>();
      if (_redPinataAnim) _redPinataBaseSprites = new List<Sprite>(_redPinataAnim.textureArray);
    }
    if (BluePinata)
    {
      _bluePinataOrigin = BluePinata.anchoredPosition;
      _bluePinataOriginalSprite = BluePinata.GetComponent<Image>()?.sprite;
      _bluePinataAnim = BluePinata.GetComponent<ImageAnimation>();
      if (_bluePinataAnim) _bluePinataBaseSprites = new List<Sprite>(_bluePinataAnim.textureArray);
    }
    if (SmallReelFrame) { _smallReelFrameOrigin = SmallReelFrame.rectTransform.anchoredPosition; SmallReelFrame.gameObject.SetActive(false); }
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

    if (GreenPinata) GreenPinata.DOAnchorPosY(GreenPinata.anchoredPosition.y + pinataScrollAmount, 0.6f).SetEase(Ease.OutBack);
    if (BluePinata) BluePinata.DOAnchorPosY(BluePinata.anchoredPosition.y + pinataScrollAmount, 0.6f).SetEase(Ease.OutBack);

    yield return new WaitForSeconds(pinataRedDelay);

    if (RedPinata)
      yield return RedPinata.DOAnchorPosY(RedPinata.anchoredPosition.y + pinataScrollAmount, 0.6f).SetEase(Ease.OutBack).WaitForCompletion();
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

  internal void UpdateLinkBonusSpinsRemaining(int count)
  {
    if (LinkBonusSpinsRemainingText) LinkBonusSpinsRemainingText.text = count.ToString();
  }

  internal void InitialiseBalanceAndWin(double balance, double bet)
  {
    if (Balance_text) Balance_text.text = balance.ToString("F3");
    if (TotalBet_text) TotalBet_text.text = bet.ToString();
    if (TotalWin_text) TotalWin_text.text = "0.000";
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

  internal void SetMeterThresholds(int greenThreshold, int redThreshold, int blueThreshold)
  {
    _greenMeterThreshold = greenThreshold;
    _redMeterThreshold = redThreshold;
    _blueMeterThreshold = blueThreshold;
  }

  internal void UpdateMeters(int green, int red, int blue)
  {
    if (GreenMeterText) GreenMeterText.text = green.ToString();
    if (RedMeterText) RedMeterText.text = red.ToString();
    if (BlueMeterText) BlueMeterText.text = blue.ToString();

    SetPinataStage(GetMeterStage(green, _greenMeterThreshold), ref _greenPinataStage, _greenPinataAnim, GreenPinataStage1Sprites, GreenPinataStage2Sprites, GreenPinata);
    SetPinataStage(GetMeterStage(red, _redMeterThreshold), ref _redPinataStage, _redPinataAnim, RedPinataStage1Sprites, RedPinataStage2Sprites, RedPinata);
    SetPinataStage(GetMeterStage(blue, _blueMeterThreshold), ref _bluePinataStage, _bluePinataAnim, BluePinataStage1Sprites, BluePinataStage2Sprites, BluePinata);
  }

  private static int GetMeterStage(int meter, int threshold)
  {
    if (threshold <= 0) return 0;
    if (meter >= threshold * 66 / 100) return 2;
    if (meter >= threshold * 33 / 100) return 1;
    return 0;
  }

  internal void PopPinata(int colorId)
  {
    RectTransform pinata = colorId == 8 ? GreenPinata : colorId == 9 ? RedPinata : BluePinata;
    if (pinata == null) return;
    pinata.DOKill();
    pinata.localScale = Vector3.one;
    pinata.DOScale(1.2f, 0.12f).SetEase(Ease.OutBack).OnComplete(() =>
      pinata.DOScale(1f, 0.1f).SetEase(Ease.InBack));
  }

  private void SetPinataStage(int targetStage, ref int stage, ImageAnimation anim, Sprite[] stage1, Sprite[] stage2, RectTransform pinata)
  {
    if (targetStage <= stage) return;
    stage = targetStage;
    Sprite[] newSprites = stage == 1 ? stage1 : stage2;
    SwapPinataAnimation(anim, newSprites);
    if (pinata)
    {
      pinata.DOKill();
      pinata.localScale = Vector3.one;
      pinata.DOScale(1.2f, 0.12f).SetEase(Ease.OutBack).OnComplete(() =>
        pinata.DOScale(1f, 0.1f).SetEase(Ease.InBack));
    }
  }

  private void SwapPinataAnimation(ImageAnimation anim, Sprite[] sprites)
  {
    if (anim == null || sprites == null || sprites.Length == 0) return;
    anim.StopAnimation();
    anim.textureArray.Clear();
    anim.textureArray.TrimExcess();
    foreach (var s in sprites) anim.textureArray.Add(s);
    anim.StartAnimation();
  }

  private void ResetPinataStage(ImageAnimation anim, List<Sprite> baseSprites, ref int stage)
  {
    stage = 0;
    if (baseSprites != null) SwapPinataAnimation(anim, baseSprites.ToArray());
  }

  internal void PlayBustedPinataOnce(string color)
  {
    ImageAnimation anim = color == "red" ? _redPinataAnim : _bluePinataAnim;
    Sprite[] sprites = color == "red" ? BustedRedPinataAnimSprites : BustedBluePinataAnimSprites;
    if (anim == null || sprites == null || sprites.Length == 0) return;
    anim.StopAnimation();
    anim.textureArray.Clear();
    anim.textureArray.TrimExcess();
    foreach (var s in sprites) anim.textureArray.Add(s);
    anim.doLoopAnimation = false;
    anim.StartAnimation();
    anim.doLoopAnimation = true;
  }

  internal IEnumerator PlayFeatureIntro(string feature)
  {
    Debug.Log($"[PlayFeatureIntro] Starting for feature: {feature}");
    GameObject introObj = null;
    ImageAnimation introAnim = null;
    Image nameGraphic = null;
    Image glowGraphic = null;

    switch (feature)
    {
      case "wheelBonus":
        introObj = GreenPinataIntroObject;
        introAnim = GreenPinataIntroAnim;
        nameGraphic = WheelBonusNameGraphic;
        glowGraphic = WheelBonusGlowGraphic;
        break;
      case "pickJackpot":
        introObj = RedPinataIntroObject;
        introAnim = RedPinataIntroAnim;
        nameGraphic = PickJackpotNameGraphic;
        glowGraphic = PickJackpotGlowGraphic;
        break;
      case "linkBonus":
        introObj = BluePinataIntroObject;
        introAnim = BluePinataIntroAnim;
        nameGraphic = LinkBonusNameGraphic;
        glowGraphic = LinkBonusGlowGraphic;
        break;
    }

    Debug.Log($"[PlayFeatureIntro] introObj={introObj?.name ?? "NULL"}, introAnim={introAnim?.name ?? "NULL"}, nameGraphic={nameGraphic?.name ?? "NULL"}");

    if (introObj) introObj.SetActive(true);
    if (introAnim)
    {
      introAnim.gameObject.SetActive(true);
      introAnim.doLoopAnimation = false;
      float shakeDelay = Mathf.Max(0f, introAnim.GetTotalDuration() - introShakeStartOffset);
      StartCoroutine(PunchGameContent(shakeDelay));
      introAnim.StartAnimation();
      StartCoroutine(PlayBatHitSounds(introAnim));
      yield return new WaitUntil(() => introAnim.IsComplete);
      introAnim.gameObject.SetActive(false);
    }

    if (nameGraphic)
    {
      nameGraphic.gameObject.SetActive(true);
      nameGraphic.transform.localScale = Vector3.zero;
      nameGraphic.color = new Color(1f, 1f, 1f, 1f);
      if (glowGraphic)
      {
        glowGraphic.gameObject.SetActive(true);
        glowGraphic.color = new Color(1f, 1f, 1f, 1f);
        glowGraphic.transform.localScale = Vector3.zero;
      }
      nameGraphic.transform.DOScale(1.2f, featureNameExpandDuration).SetEase(Ease.OutQuad);
      if (glowGraphic) glowGraphic.transform.DOScale(1.2f, featureNameExpandDuration).SetEase(Ease.OutQuad);
      yield return new WaitForSeconds(featureNameExpandDuration - featureNameFadeDuration);
      if (glowGraphic) glowGraphic.DOFade(0f, featureNameFadeDuration).OnComplete(() => glowGraphic.gameObject.SetActive(false));
      nameGraphic.DOFade(0f, featureNameFadeDuration).OnComplete(() => nameGraphic.gameObject.SetActive(false));
    }
  }

  private static readonly float[] _batHitFrames = { 43, 56, 66, 70, 72, 77, 82, 85, 89, 90, 93, 95 };

  private IEnumerator PlayBatHitSounds(ImageAnimation introAnim)
  {
    if (introAnim == null || introAnim.textureArray.Count == 0) yield break;
    float frameDelay = introAnim.GetTotalDuration() / introAnim.textureArray.Count;
    float elapsed = 0f;
    foreach (float frame in _batHitFrames)
    {
      float targetTime = frame * frameDelay;
      yield return new WaitForSeconds(targetTime - elapsed);
      elapsed = targetTime;
      if (audioManager) audioManager.PlayBatHit();
    }
  }

  private IEnumerator PunchGameContent(float delay)
  {
    if (delay > 0f) yield return new WaitForSeconds(delay);
    if (GameContent == null) yield break;
    Vector3 original = GameContent.localScale;
    yield return GameContent.DOScale(original * introScaleAmount, introScaleDownDuration).SetEase(Ease.InOutQuad).WaitForCompletion();
    yield return GameContent.DOShakeScale(introShakeDuration, introShakeStrength, introShakeVibrato).WaitForCompletion();
    yield return GameContent.DOScale(original, introScaleUpDuration).SetEase(Ease.OutBack).WaitForCompletion();
  }

  internal void LockFeatureUI(bool locked)
  {
    if (Menu_Button) Menu_Button.interactable = !locked;
    if (GameExit_Button) GameExit_Button.interactable = !locked;
  }

  internal void SetReelFrame(string feature)
  {
    if (ReelFrame == null) return;
    switch (feature)
    {
      case "wheelBonus":
      case "linkBonus":   return;
      case "pickJackpot": ReelFrame.sprite = RedReelFrameSprite;   break;
      default:            ReelFrame.sprite = DefaultReelFrameSprite; break;
    }
  }

  internal IEnumerator SlideContentDown()
  {
    if (ReelFrame) ReelFrame.rectTransform.DOAnchorPosY(ReelFrame.rectTransform.anchoredPosition.y - 275f, slideContentDuration).SetEase(Ease.InOutCubic);
    if (MoveUpAfterIntro) MoveUpAfterIntro.DOAnchorPosY(MoveUpAfterIntro.anchoredPosition.y - 325f, slideContentDuration).SetEase(Ease.InOutCubic);
    yield return GameContent.DOAnchorPosY(GameContent.anchoredPosition.y - featureHideAmount, slideContentDuration)
      .SetEase(Ease.InOutCubic).WaitForCompletion();
    // Reset pinatas and small frame to pre-intro local Y while hidden, so SlideContentUp can animate them back up
    if (GreenPinata) GreenPinata.anchoredPosition = _greenPinataOrigin;
    if (BluePinata) BluePinata.anchoredPosition = _bluePinataOrigin;
    if (RedPinata) RedPinata.anchoredPosition = new Vector2(RedPinata.anchoredPosition.x, _redPinataOrigin.y);
    if (SmallReelFrame && SmallReelFrame.gameObject.activeSelf)
      SmallReelFrame.rectTransform.anchoredPosition = _smallReelFrameOrigin;
  }

  internal IEnumerator SlideContentUp()
  {
    if (MoveUpAfterIntro) MoveUpAfterIntro.DOAnchorPosY(MoveUpAfterIntro.anchoredPosition.y + 325f, 0.5f).SetEase(Ease.OutCubic);
    float slideDuration = 0.5f;
    if (ReelFrame) ReelFrame.rectTransform.DOAnchorPosY(ReelFrame.rectTransform.anchoredPosition.y + 275f, slideDuration).SetEase(Ease.OutCubic);
    GameContent.DOAnchorPosY(GameContent.anchoredPosition.y + featureHideAmount, slideDuration).SetEase(Ease.OutCubic);
    yield return new WaitForSeconds(Mathf.Max(0f, slideDuration - pinataEarlyStart));
    if (GreenPinata) GreenPinata.DOAnchorPosY(GreenPinata.anchoredPosition.y + pinataScrollAmount, 0.6f).SetEase(Ease.OutBack);
    if (BluePinata) BluePinata.DOAnchorPosY(BluePinata.anchoredPosition.y + pinataScrollAmount, 0.6f).SetEase(Ease.OutBack);
    yield return new WaitForSeconds(pinataRedDelay);
    if (SmallReelFrame && SmallReelFrame.gameObject.activeSelf)
      SmallReelFrame.rectTransform.DOAnchorPosY(SmallReelFrame.rectTransform.anchoredPosition.y + pinataScrollAmount, 0.6f).SetEase(Ease.OutBack);
    if (RedPinata)
      yield return RedPinata.DOAnchorPosY(RedPinata.anchoredPosition.y + pinataScrollAmount, 0.6f).SetEase(Ease.OutBack).WaitForCompletion();
  }

  internal void SetupFeaturePinata(string feature)
  {
    if (feature == "pickJackpot" && RedPinata)
    {
      if (_redPinataAnim) _redPinataAnim.StopAnimation();
      Image img = RedPinata.GetComponent<Image>();
      if (img) img.enabled = false;
      RedPinata.anchoredPosition = new Vector2(0f, RedPinata.anchoredPosition.y);
      if (BustedRedPinata) BustedRedPinata.SetActive(true);
    }
    else if (feature == "linkBonus" && BluePinata)
    {
      if (_bluePinataAnim) _bluePinataAnim.StopAnimation();
      Image img = BluePinata.GetComponent<Image>();
      if (img && BustedBluePinataSprite) img.sprite = BustedBluePinataSprite;
      BluePinata.anchoredPosition = new Vector2(0f, BluePinata.anchoredPosition.y);
      if (SmallReelFrame) { SmallReelFrame.sprite = SpinsRemainingFrameSprite; SmallReelFrame.gameObject.SetActive(true); }
      if (GreenPinata) GreenPinata.gameObject.SetActive(false);
      if (RedPinata) RedPinata.gameObject.SetActive(false);
      if (SlotMain) SlotMain.SetActive(false);
      if (AnimationLayer) AnimationLayer.SetActive(false);
      if (LinkBonusFeature) LinkBonusFeature.SetActive(true);
    }
  }

  internal void CleanupFeaturePinata(string feature)
  {
    if (SmallReelFrame) { SmallReelFrame.rectTransform.anchoredPosition = _smallReelFrameOrigin; SmallReelFrame.gameObject.SetActive(false); }
    if (feature == "wheelBonus")
    {
      ResetPinataStage(_greenPinataAnim, _greenPinataBaseSprites, ref _greenPinataStage);
    }
    else if (feature == "pickJackpot" && RedPinata)
    {
      if (BustedRedPinata) BustedRedPinata.SetActive(false);
      Image img = RedPinata.GetComponent<Image>();
      if (img) img.enabled = true;
      RedPinata.anchoredPosition = new Vector2(_redPinataOrigin.x, RedPinata.anchoredPosition.y);
      ResetPinataStage(_redPinataAnim, _redPinataBaseSprites, ref _redPinataStage);
    }
    else if (feature == "linkBonus" && BluePinata)
    {
      Image img = BluePinata.GetComponent<Image>();
      if (img && _bluePinataOriginalSprite) img.sprite = _bluePinataOriginalSprite;
      BluePinata.anchoredPosition = new Vector2(_bluePinataOrigin.x, BluePinata.anchoredPosition.y);
      ResetPinataStage(_bluePinataAnim, _bluePinataBaseSprites, ref _bluePinataStage);
      if (GreenPinata) GreenPinata.gameObject.SetActive(true);
      if (RedPinata) RedPinata.gameObject.SetActive(true);
      if (SlotMain) SlotMain.SetActive(true);
      if (AnimationLayer) AnimationLayer.SetActive(true);
      if (LinkBonusFeature) LinkBonusFeature.SetActive(false);
    }
  }

  internal void InitialiseUI(List<double> bets, List<Symbol> symbols)
  {
    betAmounts = bets;
    foreach (var symbol in symbols)
    {
      if (symbol.id >= 3 && symbol.id <= 7 && symbol.multiplier?[0] != null)
        jackpotMultipliers[symbol.id - 3] = symbol.multiplier[0].Value;
    }
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

  internal void ShowPickJackpotScreen()
  {
    PickJackpotSelected = false;
    _selectedPinataIndex = -1;

    if (PickJackpotPanel)
    {
      PickJackpotPanel.GetComponent<RectTransform>().anchoredPosition = _pickJackpotPanelOrigin;
      PickJackpotPanel.SetActive(true);
      if (PickingJackpotGroup)
      {
        PickingJackpotGroup.alpha = 0f;
        PickingJackpotGroup.DOFade(1f, 1f);
        PickJackpotPanel.transform.localScale = Vector3.one * 0.9f;
        PickJackpotPanel.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutQuad);
      }
    }

    if (JackpotRevealImages != null)
      for (int i = 0; i < JackpotRevealImages.Length; i++)
      {
        JackpotRevealImages[i].gameObject.SetActive(false);
        JackpotRevealImages[i].transform.localScale = Vector3.one;
        if (_jackpotRevealImageOrigins != null)
          JackpotRevealImages[i].rectTransform.anchoredPosition = _jackpotRevealImageOrigins[i];
      }

    for (int i = 0; i < PinataButtons.Length; i++)
    {
      int index = i;
      Image btnImg = PinataButtons[i].GetComponent<Image>();
      if (btnImg) btnImg.color = Color.white;
      PinataButtons[i].onClick.RemoveAllListeners();
      PinataButtons[i].onClick.AddListener(() => OnPinataSelected(index));
      PinataButtons[i].interactable = true;
    }

    if (audioManager) audioManager.PlayInsideJackpot();

    if (_pickJackpotTimerRoutine != null) StopCoroutine(_pickJackpotTimerRoutine);
    _pickJackpotTimerRoutine = StartCoroutine(PickJackpotTimer());

    foreach (var anim in PinataButtonAnimations)
      if (anim != null) anim.StartAnimation();
    if (TimerClockAnimation)
    {
      if (TimerClockAnimation.rendererDelegate) TimerClockAnimation.rendererDelegate.color = Color.white;
      TimerClockAnimation.StartAnimation();
    }
  }

  private void OnPinataSelected(int index)
  {
    if (PickJackpotSelected) return;
    _selectedPinataIndex = index;
    if (_pickJackpotTimerRoutine != null)
    {
      StopCoroutine(_pickJackpotTimerRoutine);
      _pickJackpotTimerRoutine = null;
    }
    foreach (var btn in PinataButtons) btn.interactable = false;
    foreach (var anim in PinataButtonAnimations)
      if (anim != null) anim.StopAnimation();
    if (TimerClockAnimation && TimerClockAnimation.rendererDelegate)
      TimerClockAnimation.rendererDelegate.DOFade(0f, 0.3f);
    if (audioManager) audioManager.PlayChoosedJackpot();
    PickJackpotSelected = true;
  }

  private IEnumerator PickJackpotTimer()
  {
    float timeRemaining = pickJackpotTimerDuration;
    while (timeRemaining > 0)
    {
      timeRemaining -= Time.deltaTime;
      if (PickJackpotTimerText) PickJackpotTimerText.text = Mathf.CeilToInt(timeRemaining).ToString();
      yield return null;
    }
    OnPinataSelected(UnityEngine.Random.Range(0, PinataButtons.Length));
  }

  internal IEnumerator RevealJackpot(string goalJackpot)
  {
    List<string> remaining = new List<string>(_jackpotOrder);
    remaining.Remove(goalJackpot);

    for (int i = remaining.Count - 1; i > 0; i--)
    {
      int j = UnityEngine.Random.Range(0, i + 1);
      string temp = remaining[i];
      remaining[i] = remaining[j];
      remaining[j] = temp;
    }

    string[] assignment = new string[PinataButtons.Length];
    assignment[_selectedPinataIndex] = goalJackpot;
    int remainingIndex = 0;
    for (int i = 0; i < assignment.Length; i++)
    {
      if (i == _selectedPinataIndex) continue;
      assignment[i] = remaining[remainingIndex++];
    }

    for (int i = 0; i < JackpotRevealImages.Length; i++)
    {
      JackpotRevealImages[i].sprite = GetJackpotSprite(assignment[i]);
      JackpotRevealImages[i].gameObject.SetActive(true);
      JackpotRevealImages[i].color = new Color(1, 1, 1, 0);
    }

    List<Tween> fadeTweens = new List<Tween>();
    for (int i = 0; i < PinataButtons.Length; i++)
    {
      Image pinataImg = PinataButtons[i].GetComponent<Image>();
      if (pinataImg) fadeTweens.Add(pinataImg.DOFade(0f, 0.5f));
    }
    for (int i = 0; i < JackpotRevealImages.Length; i++)
    {
      float targetAlpha = i == _selectedPinataIndex ? 1f : 0.4f;
      fadeTweens.Add(JackpotRevealImages[i].DOFade(targetAlpha, 0.5f));
    }
    yield return fadeTweens[^1].WaitForCompletion();

    yield return JackpotRevealImages[_selectedPinataIndex].transform
      .DOScale(1.3f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();

    yield return new WaitForSeconds(1f);

    if (FallingJackpotImage)
    {
      FallingJackpotImage.sprite = GetJackpotSprite(goalJackpot);
      FallingJackpotImage.preserveAspect = true;
    }

    for (int i = 0; i < JackpotRevealImages.Length; i++)
      if (i != _selectedPinataIndex)
        JackpotRevealImages[i].DOFade(0f, 0.5f);

    RectTransform selectedRT = JackpotRevealImages[_selectedPinataIndex].rectTransform;
    yield return selectedRT.DOAnchorPos(jackpotCenterTarget, 0.5f).SetEase(Ease.InOutCubic).WaitForCompletion();

    for (int i = 0; i < JackpotRevealImages.Length; i++)
      if (i != _selectedPinataIndex)
        JackpotRevealImages[i].gameObject.SetActive(false);

    if (FreeSpinsUntilImage) FreeSpinsUntilImage.gameObject.SetActive(true);

    if (PickingJackpotGroup)
      yield return PickingJackpotGroup.DOFade(0f, jackpotTextFadeDuration).WaitForCompletion();

    if (JackpotPickedObject) JackpotPickedObject.SetActive(true);
    if (JackpotPickedGroup)
      yield return JackpotPickedGroup.DOFade(1f, jackpotTextFadeDuration).WaitForCompletion();

    yield return new WaitForSeconds(2f);

    if (FallingJackpotRT)
    {
      FallingJackpotRT.gameObject.SetActive(true);
      FallingJackpotRT.anchoredPosition = Vector2.zero;
      FallingJackpotRT.localEulerAngles = Vector3.zero;
    }

    // panel exits fast, don't await so jackpot animates simultaneously
    if (PickJackpotPanel)
      PickJackpotPanel.GetComponent<RectTransform>()
        .DOAnchorPosY(PickJackpotPanel.GetComponent<RectTransform>().anchoredPosition.y + offscreenOffset, 0.4f)
        .SetEase(Ease.InBack);

    if (FallingJackpotRT)
    {
      // launch up near top while rotating 180° CW
      yield return DOTween.Sequence()
        .Append(FallingJackpotRT.DOAnchorPosY(jackpotLaunchHeight, 0.6f).SetEase(Ease.OutQuad))
        .Join(FallingJackpotRT.DOLocalRotate(new Vector3(0, 0, -180), 0.6f, RotateMode.LocalAxisAdd))
        .WaitForCompletion();

      // at apex: reels slide back up in parallel with the jackpot falling
      StartCoroutine(SlideContentUp());

      // fall to landing point while rotating another 180° CW, ends right-side up
      float landingY = JackpotLandingPoint != null
        ? ((RectTransform)JackpotLandingPoint).anchoredPosition.y : 0;
      yield return DOTween.Sequence()
        .Append(FallingJackpotRT.DOAnchorPosY(landingY, 0.5f).SetEase(Ease.InQuad))
        .Join(FallingJackpotRT.DOLocalRotate(new Vector3(0, 0, -180), 0.5f, RotateMode.LocalAxisAdd))
        .WaitForCompletion();
    }

    if (PickJackpotPanel) PickJackpotPanel.SetActive(false);
  }

  private Sprite GetJackpotSprite(string jackpotName)
  {
    switch (jackpotName)
    {
      case "mini": return MiniJackpotSprite;
      case "minor": return MinorJackpotSprite;
      case "major": return MajorJackpotSprite;
      case "mega": return MegaJackpotSprite;
      case "grand": return GrandJackpotSprite;
      default: return null;
    }
  }

  private Sprite GetJackpotWinSprite(string tier)
  {
    switch (tier)
    {
      case "mini": return MiniWinSprite;
      case "minor": return MinorWinSprite;
      case "major": return MajorWinSprite;
      case "mega": return MegaWinSprite;
      case "grand": return GrandWinSprite;
      default: return null;
    }
  }

  internal IEnumerator ShowJackpotWinSequence(string tier, double jackpotAmount, double totalWin)
  {
    if (JackpotWinSequencePanel) JackpotWinSequencePanel.SetActive(true);
    yield return null;
    if (CashFallingAnim) { CashFallingAnim.doLoopAnimation = true; CashFallingAnim.StopAnimation(); CashFallingAnim.StartAnimation(); }
    if (CoinFallingAnim) { CoinFallingAnim.doLoopAnimation = true; CoinFallingAnim.StopAnimation(); CoinFallingAnim.StartAnimation(); }
    
    if (audioManager)
    {
      if (tier == "mini") audioManager.PlayMiniJackpot();
      else if (tier == "minor") audioManager.PlayMinorJackpot();
      else if (tier == "major") audioManager.PlayMajorJackpot();
      else if (tier == "mega") audioManager.PlayMegaJackpot();
      else if (tier == "grand") audioManager.PlayGrandJackpot();
    }

    if (JackpotWinGraphicImage) JackpotWinGraphicImage.sprite = GetJackpotWinSprite(tier);
    if (JackpotWinGraphic)
    {
      JackpotWinGraphic.gameObject.SetActive(true);
      float targetY = JackpotWinGraphic.anchoredPosition.y;
      JackpotWinGraphic.anchoredPosition = new Vector2(JackpotWinGraphic.anchoredPosition.x, targetY + offscreenOffset);
      yield return JackpotWinGraphic.DOAnchorPosY(targetY, jackpotGraphicDropDuration).SetEase(Ease.OutCubic).WaitForCompletion();
    }

    if (JackpotAmountPanel)
    {
      JackpotAmountPanel.transform.localScale = Vector3.one * 0.05f;
      JackpotAmountPanel.SetActive(true);
      JackpotAmountPanel.transform.DOScale(Vector3.one, jackpotPanelExpandDuration).SetEase(Ease.OutBack);
    }

    float jackpotDisplay = 0f;
    if (JackpotWinAmountText)
      yield return DOTween.To(() => jackpotDisplay, v => { jackpotDisplay = v; JackpotWinAmountText.text = v.ToString("F3"); },
        (float)jackpotAmount, jackpotCountDuration).WaitForCompletion();
    else
      yield return new WaitForSeconds(jackpotCountDuration);

    yield return new WaitForSeconds(jackpotHoldDuration);

    if (JackpotWinGraphic) JackpotWinGraphic.gameObject.SetActive(false);
    if (JackpotAmountPanel) JackpotAmountPanel.SetActive(false);
    if (CashFallingAnim) { CashFallingAnim.StopAnimation(); CashFallingAnim.doLoopAnimation = false; }
    if (CoinFallingAnim) { CoinFallingAnim.StopAnimation(); CoinFallingAnim.doLoopAnimation = false; }
    if (JackpotWinSequencePanel) JackpotWinSequencePanel.SetActive(false);
    if (FallingJackpotRT) FallingJackpotRT.gameObject.SetActive(false);
    if (JackpotPickedObject) JackpotPickedObject.SetActive(false);
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
    if (MiniPayoutText) MiniPayoutText.text = (bet * jackpotMultipliers[0]).ToString("F2");
    if (MinorPayoutText) MinorPayoutText.text = (bet * jackpotMultipliers[1]).ToString("F2");
    if (MajorPayoutText) MajorPayoutText.text = (bet * jackpotMultipliers[2]).ToString("F2");
    if (MegaPayoutText) MegaPayoutText.text = (bet * jackpotMultipliers[3]).ToString("F2");
    if (GrandPayoutText) GrandPayoutText.text = (bet * jackpotMultipliers[4]).ToString("F2");
    if (WheelBonusMiniText) WheelBonusMiniText.text = (bet * jackpotMultipliers[0]).ToString("F2");
    if (WheelBonusMinorText) WheelBonusMinorText.text = (bet * jackpotMultipliers[1]).ToString("F2");
    if (WheelBonusMajorText) WheelBonusMajorText.text = (bet * jackpotMultipliers[2]).ToString("F2");
    if (WheelBonusMegaText) WheelBonusMegaText.text = (bet * jackpotMultipliers[3]).ToString("F2");
    if (WheelBonusGrandText) WheelBonusGrandText.text = (bet * jackpotMultipliers[4]).ToString("F2");
  }

  private void ShowSlide(int index)
  {
    if (SlideContainer && InfoSlides != null && InfoSlides.Length > 0)
      SlideContainer.sprite = InfoSlides[index];
  }

}
