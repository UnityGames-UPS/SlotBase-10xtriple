using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;
using System.Reflection;

public class SlotBehaviour : MonoBehaviour
{
  [Header("Sprites")]
  [SerializeField]
  private Sprite[] myImages;  //images taken initially

  [Header("Slot Images")]
  [SerializeField]
  private List<SlotImage> TempImages;     //class to store the result matrix

  [Header("Slots Transforms")]
  [SerializeField]
  private Transform[] Slot_Transform;

  [Header("Line Button Objects")]
  [SerializeField]
  private List<GameObject> StaticLine_Objects;

  [Header("Line Button Texts")]
  [SerializeField]
  private List<TMP_Text> StaticLine_Texts;

  [Header("Payline Graphics")]
  [SerializeField]
  private List<GameObject> PaylineGraphics;

  private int _hoverLineIndex = -1;

  [Header("Buttons")]
  [SerializeField]
  private Button Spin_Button;
  [SerializeField]
  private Button AutoSpin_Button;
  [SerializeField] private Button AutoSpinStop_Button;
  [SerializeField]
  private Button MaxBet_Button;
  [SerializeField]
  private Button TBetPlus_Button;
  [SerializeField]
  private Button TBetMinus_Button;
  [SerializeField] private Button Turbo_Button;
  [SerializeField] private Sprite SpinSprite;
  [SerializeField] private Sprite StopSprite;

  [Header("Animated Sprites")]
  [SerializeField]
  private Sprite[] Blank_Sprite;
  [SerializeField]
  private Sprite[] SingleBar_Sprite;
  [SerializeField]
  private Sprite[] DoubleBar_Sprite;
  [SerializeField]
  private Sprite[] TripleBar_Sprite;
  [SerializeField]
  private Sprite[] Bell_Sprite;
  [SerializeField]
  private Sprite[] Red7_Sprite;
  [SerializeField]
  private Sprite[] Wild2x_Sprite;
  [SerializeField]
  private Sprite[] Wild3x_Sprite;
  [SerializeField]
  private Sprite[] Wild5x_Sprite;
  [SerializeField]
  private Sprite[] Wild10x_Sprite;
  [SerializeField]
  private Sprite[] Scatter_Sprite;

  [Header("Miscellaneous UI")]
  [SerializeField]
  private TMP_Text Balance_text;
  [SerializeField]
  private TMP_Text TotalBet_text;
  [SerializeField]
  private TMP_Text LineBet_text;
  [SerializeField]
  private TMP_Text TotalWin_text;

  [Header("Audio Management")]
  [SerializeField]
  private AudioManager audioController;

  [SerializeField]
  private UIManager uiManager;

  [Header("Free Spin Trigger Anticipation")]
  [SerializeField] private Camera anticipationCamera;
  [SerializeField] private float anticipationStopDuration = 1.5f;
  [SerializeField] private float anticipationZoomAmount = 0.8f;
  [SerializeField] private float anticipationZoomInDuration = 0.6f;
  [SerializeField] private float anticipationZoomOutDuration = 0.9f;

  [Header("Free Spin Special Reel")]
  [SerializeField] private GameObject SpecialReelObject;
  [SerializeField] private Transform SpecialReelTransform;
  [SerializeField] private float specialReelSpeedMultiplier = 0.4f;
  [SerializeField] private float specialReelDuration = 2f;
  [SerializeField] private GameObject MiddleReelGlow;

  int tweenHeight = 0;  //calculate the height at which tweening is done

  [SerializeField]
  private GameObject Image_Prefab;    //icons prefab
  [SerializeField] Sprite[] TurboToggleSprites;

  private List<Tweener> alltweens = new List<Tweener>();

  [SerializeField]
  private List<ImageAnimation> TempList;  //stores the sprites whose animation is running at present 

  [SerializeField]
  private SocketIOManager SocketManager;

  private Coroutine AutoSpinRoutine = null;
  private Coroutine FreeSpinRoutine = null;
  private Coroutine tweenroutine;
  private Tween BalanceTween;
  internal bool IsAutoSpin = false;
  internal bool IsFreeSpin = false;
  private bool IsSpinning = false;
  private bool CheckSpinAudio = false;
  internal bool CheckPopups = false;
  internal int BetCounter = 0;
  private double currentBalance = 0;
  private double currentTotalBet = 0;
  protected int Lines = 5;
  [SerializeField]
  private int IconSizeFactor = 100;       //set this parameter according to the size of the icon and spacing
  private int numberOfSlots = 3;          //number of columns
  private int numberOfRows = 5;           //number of rows per column (3 real + 2 decorative edge rows)
  private bool StopSpinToggle;
  private float SpinDelay = 0.2f;
  private bool IsTurboOn;
  internal bool WasAutoSpinOn;
  internal bool socketConnected = false;
  private int[,] initialMatrix = new int[,]
  {
    { 4, 4, 5 },
    { 0, 0, 0 },
    { 10, 10, 10 },
    { 0, 0, 0 },
    { 3, 3, 3 }
  };

  private void Start()
  {
    IsAutoSpin = false;

    Debug.Log($"[SlotBehaviour.Start] Spin_Button is {(Spin_Button ? Spin_Button.name : "NULL")}");
    if (Spin_Button) Spin_Button.onClick.RemoveAllListeners();
    if (Spin_Button) Spin_Button.onClick.AddListener(OnSpinButtonPressed);

    if (TBetPlus_Button) TBetPlus_Button.onClick.RemoveAllListeners();
    if (TBetPlus_Button) TBetPlus_Button.onClick.AddListener(delegate { ChangeBet(true); });

    if (TBetMinus_Button) TBetMinus_Button.onClick.RemoveAllListeners();
    if (TBetMinus_Button) TBetMinus_Button.onClick.AddListener(delegate { ChangeBet(false); });

    if (MaxBet_Button) MaxBet_Button.onClick.RemoveAllListeners();
    if (MaxBet_Button) MaxBet_Button.onClick.AddListener(MaxBet);

    if (AutoSpin_Button) AutoSpin_Button.onClick.RemoveAllListeners();
    if (AutoSpin_Button) AutoSpin_Button.onClick.AddListener(AutoSpin);

    if (Turbo_Button) Turbo_Button.onClick.RemoveAllListeners();
    if (Turbo_Button) Turbo_Button.onClick.AddListener(TurboToggle);

    if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.RemoveAllListeners();
    if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.AddListener(StopAutoSpin);

    tweenHeight = (15 * IconSizeFactor) - 280;
  }

  void TurboToggle()
  {
    audioController.PlayButton();
    if (IsTurboOn)
    {
      IsTurboOn = false;
      Turbo_Button.GetComponent<ImageAnimation>().StopAnimation();
      Turbo_Button.image.sprite = TurboToggleSprites[0];
      Turbo_Button.image.color = new Color(0.86f, 0.86f, 0.86f, 1);
    }
    else
    {
      IsTurboOn = true;
      Turbo_Button.GetComponent<ImageAnimation>().StartAnimation();
      Turbo_Button.image.color = new Color(1, 1, 1, 1);
    }
  }

  #region Autospin
  private void AutoSpin()
  {
    if (IsAutoSpin || IsSpinning)
    {
      return;
    }
    IsAutoSpin = true;
    if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(true);
    if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(false);

    if (AutoSpinRoutine != null)
    {
      StopCoroutine(AutoSpinRoutine);
      AutoSpinRoutine = null;
    }
    AutoSpinRoutine = StartCoroutine(AutoSpinCoroutine());
  }

  private void StopAutoSpin()
  {
    audioController.PlayButton();
    if (IsAutoSpin)
    {
      IsAutoSpin = false;
      if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(false);
      if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(true);
      StartCoroutine(StopAutoSpinCoroutine());
    }
  }

  private IEnumerator AutoSpinCoroutine()
  {
    while (IsAutoSpin)
    {
      yield return new WaitUntil(() => !CheckPopups);
      StartSlots(IsAutoSpin);
      yield return tweenroutine;
      yield return new WaitForSeconds(SpinDelay);
    }
    WasAutoSpinOn = false;
  }

  private IEnumerator StopAutoSpinCoroutine()
  {
    yield return new WaitUntil(() => !IsSpinning);
    ToggleButtonGrp(true);
    if (AutoSpinRoutine != null || tweenroutine != null)
    {
      StopCoroutine(AutoSpinRoutine);
      StopCoroutine(tweenroutine);
      tweenroutine = null;
      AutoSpinRoutine = null;
      StopCoroutine(StopAutoSpinCoroutine());
    }
  }
  #endregion

  #region FreeSpin
  internal void FreeSpin(int spins)
  {
    if (!IsFreeSpin)
    {
      uiManager.UpdateFreeSpinsRemaining(spins);
      IsFreeSpin = true;
      ToggleButtonGrp(false);

      if (FreeSpinRoutine != null)
      {
        StopCoroutine(FreeSpinRoutine);
        FreeSpinRoutine = null;
      }
      FreeSpinRoutine = StartCoroutine(FreeSpinCoroutine(spins));
    }
  }

  private IEnumerator FreeSpinCoroutine(int spinchances)
  {
    yield return new WaitForSecondsRealtime(1.5f);
    uiManager.UpdateFreeSpinsRemaining(spinchances);
    bool isFreeSpinActive;
    do
    {
      StartSlots();
      yield return tweenroutine;
      yield return new WaitForSeconds(SpinDelay);
      isFreeSpinActive = SocketManager.ResultData.payload.isFreeSpinActive;
      uiManager.UpdateFreeSpinsRemaining(SocketManager.ResultData.payload.freeSpinsRemaining);
    } while (isFreeSpinActive);
    if (MiddleReelGlow) MiddleReelGlow.SetActive(false);
    uiManager.EndFreeSpinTriggerSequence();

    double totalFreeSpinWin = SocketManager.ResultData.payload.totalFreeSpinWin;
    StartCoroutine(uiManager.ShowSpinWin(totalFreeSpinWin));
    StartCoroutine(uiManager.ShowBonusWinSequence(totalFreeSpinWin, currentTotalBet));

    if (WasAutoSpinOn)
    {
      AutoSpin();
    }
    else
    {
      ToggleButtonGrp(true);
    }
    IsFreeSpin = false;
  }
  #endregion

  private void CompareBalance()
  {
    if (currentBalance < currentTotalBet)
    {
      uiManager.LowBalPopup();
    }
  }

  #region LinesCalculation
  //Fetch Lines from backend
  internal void FetchLines(string LineVal, int count)
  {
    if (StaticLine_Texts.Count > count) StaticLine_Texts[count].text = (count + 1).ToString();
    if (StaticLine_Objects.Count > count) StaticLine_Objects[count].SetActive(true);
  }

  //Generate Static Lines from button hovers
  internal void GenerateStaticLine(TMP_Text LineID_Text)
  {
    DestroyStaticLine();
    int LineID = 1;
    try
    {
      LineID = int.Parse(LineID_Text.text);
    }
    catch (Exception e)
    {
      Debug.Log("Exception while parsing " + e.Message);
    }
    int index = LineID - 1;
    if (PaylineGraphics.Count > index)
    {
      PaylineGraphics[index].SetActive(true);
      StartGameAnimation(PaylineGraphics[index]);
      _hoverLineIndex = index;
    }
  }

  //Destroy Static Lines from button hovers
  internal void DestroyStaticLine()
  {
    if (_hoverLineIndex >= 0 && PaylineGraphics.Count > _hoverLineIndex)
    {
      PaylineGraphics[_hoverLineIndex].SetActive(false);
    }
    _hoverLineIndex = -1;
  }
  #endregion

  private void MaxBet()
  {
    if (audioController) audioController.PlayButton();
    BetCounter = SocketManager.InitialData.bets.Count - 1;
    if (LineBet_text) LineBet_text.text = SocketManager.InitialData.bets[BetCounter].ToString();
    if (TotalBet_text) TotalBet_text.text = (SocketManager.InitialData.bets[BetCounter] * Lines).ToString();
    currentTotalBet = SocketManager.InitialData.bets[BetCounter] * Lines;

  }

  private void ChangeBet(bool IncDec)
  {
    if (audioController) audioController.PlayButton();
    if (IncDec)
    {
      BetCounter++;
      if (BetCounter >= SocketManager.InitialData.bets.Count)
      {
        BetCounter = 0; // Loop back to the first bet
      }
    }
    else
    {
      BetCounter--;
      if (BetCounter < 0)
      {
        BetCounter = SocketManager.InitialData.bets.Count - 1; // Loop to the last bet
      }
    }
    if (LineBet_text) LineBet_text.text = SocketManager.InitialData.bets[BetCounter].ToString();
    if (TotalBet_text) TotalBet_text.text = (SocketManager.InitialData.bets[BetCounter] * Lines).ToString();
    currentTotalBet = SocketManager.InitialData.bets[BetCounter] * Lines;
    uiManager.InitialiseUI(SocketManager.InitialData.bets, SocketManager.UIData.paylines.symbols);
  }

  #region InitialFunctions
  // internal void shuffleInitialMatrix()
  // {
  //   for (int i = 0; i < TempImages.Count; i++)
  //   {
  //     for (int j = 0; j < 3; j++)
  //     {
  //       int randomIndex = UnityEngine.Random.Range(0, 14);
  //       TempImages[i].slotImages[j].sprite = myImages[randomIndex];
  //     }
  //   }
  // }


  internal void InitializeMatrix()
  {
    for (int row = 0; row < initialMatrix.GetLength(0); row++)
    {
      for (int col = 0; col < initialMatrix.GetLength(1); col++)
      {
        int val = initialMatrix[row, col];

        TempImages[col].slotImages[row].sprite = myImages[val];

        ImageAnimation animScript = TempImages[col].slotImages[row].GetComponent<ImageAnimation>();
        if (animScript != null)
        {
          PopulateAnimationSprites(animScript, val);

          animScript.StartAnimation();
          TempList.Add(animScript);
        }
      }
    }
  }


  internal void SetInitialUI()
  {
    socketConnected = true;
    BetCounter = 0;
    Lines = SocketManager.InitialData.totalLines;
    if (LineBet_text) LineBet_text.text = SocketManager.InitialData.bets[BetCounter].ToString();
    if (TotalBet_text) TotalBet_text.text = (SocketManager.InitialData.bets[BetCounter] * Lines).ToString();
    if (TotalWin_text) TotalWin_text.text = "0.000";
    if (Balance_text) Balance_text.text = SocketManager.PlayerData.balance.ToString("F3");
    currentBalance = SocketManager.PlayerData.balance;
    currentTotalBet = SocketManager.InitialData.bets[BetCounter] * Lines;
    CompareBalance();
    uiManager.InitialiseUI(SocketManager.InitialData.bets, SocketManager.UIData.paylines.symbols);
  }
  #endregion

  private void OnApplicationFocus(bool focus)
  {
  }

  //function to populate animation sprites accordingly
  private void PopulateAnimationSprites(ImageAnimation animScript, int val)
  {
    animScript.textureArray.Clear();
    animScript.textureArray.TrimExcess();
    switch (val)
    {
      case 0:
        for (int i = 0; i < Blank_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Blank_Sprite[i]);
        }
        animScript.AnimationSpeed = 12f;
        break;
      case 1:
        for (int i = 0; i < SingleBar_Sprite.Length; i++)
        {
          animScript.textureArray.Add(SingleBar_Sprite[i]);
        }
        animScript.AnimationSpeed = 12f;
        break;
      case 2:
        for (int i = 0; i < DoubleBar_Sprite.Length; i++)
        {
          animScript.textureArray.Add(DoubleBar_Sprite[i]);
        }
        animScript.AnimationSpeed = 12f;
        break;
      case 3:
        for (int i = 0; i < TripleBar_Sprite.Length; i++)
        {
          animScript.textureArray.Add(TripleBar_Sprite[i]);
        }
        animScript.AnimationSpeed = 12f;
        break;
      case 4:
        for (int i = 0; i < Bell_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Bell_Sprite[i]);
        }
        animScript.AnimationSpeed = 12f;
        break;
      case 5:
        for (int i = 0; i < Red7_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Red7_Sprite[i]);
        }
        animScript.AnimationSpeed = 12f;
        break;
      case 6:
        for (int i = 0; i < Wild2x_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Wild2x_Sprite[i]);
        }
        animScript.AnimationSpeed = 30f;
        break;
      case 7:
        for (int i = 0; i < Wild3x_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Wild3x_Sprite[i]);
        }
        animScript.AnimationSpeed = 30f;
        break;
      case 8:
        for (int i = 0; i < Wild5x_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Wild5x_Sprite[i]);
        }
        animScript.AnimationSpeed = 30f;
        break;
      case 9:
        for (int i = 0; i < Wild10x_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Wild10x_Sprite[i]);
        }
        animScript.AnimationSpeed = 30f;
        break;
      case 10:
        for (int i = 0; i < Scatter_Sprite.Length; i++)
        {
          animScript.textureArray.Add(Scatter_Sprite[i]);
        }
        animScript.AnimationSpeed = 30f;
        break;
    }
  }

  #region SlotSpin
  private void OnSpinButtonPressed()
  {
    Debug.Log($"[SpinButton] Clicked. IsSpinning={IsSpinning}");
    if (audioController) audioController.PlayButton();
    if (IsSpinning)
      OnStopSpinPressed();
    else
      StartSlots();
  }

  private void OnStopSpinPressed()
  {
    StopSpinToggle = true;
  }

  //starts the spin process
  private void StartSlots(bool autoSpin = false)
  {

    if (TotalWin_text && !IsFreeSpin) TotalWin_text.text = "0.000";

    if (!autoSpin)
    {
      if (AutoSpinRoutine != null)
      {
        StopCoroutine(AutoSpinRoutine);
        StopCoroutine(tweenroutine);
        tweenroutine = null;
        AutoSpinRoutine = null;
      }
    }
    if (TempList.Count > 0)
    {
      StopGameAnimation();
    }
    for (int i = 0; i < PaylineGraphics.Count; i++)
    {
      PaylineGraphics[i].SetActive(false);
    }
    tweenroutine = StartCoroutine(TweenRoutine());
  }

  //manage the Routine for spinning of the slots
  private IEnumerator TweenRoutine()
  {
    if (currentBalance < currentTotalBet && !IsFreeSpin)
    {
      CompareBalance();
      StopAutoSpin();
      yield return new WaitForSeconds(1);
      ToggleButtonGrp(true);
      yield break;
    }
    CheckSpinAudio = true;

    IsSpinning = true;

    ToggleButtonGrp(false);
    if (Spin_Button) Spin_Button.GetComponent<Image>().sprite = StopSprite;
    for (int i = 0; i < numberOfSlots; i++)
    {
      InitializeTweening(Slot_Transform[i]);
      yield return new WaitForSeconds(0.1f);
    }

    if (!IsFreeSpin)
    {
      BalanceDeduction();
    }

    SocketManager.AccumulateResult(BetCounter);
    yield return new WaitUntil(() => SocketManager.isResultdone);

    for (int i = 0; i < numberOfRows; i++)
    {
      for (int j = 0; j < numberOfSlots; j++)
      {
        int resultNum = int.Parse(SocketManager.ResultData.matrix[i][j]);
        // print("resultNum: " + resultNum);
        // print("image loc: " + j + " " + i);
        ImageAnimation animScript = TempImages[j].slotImages[i].GetComponent<ImageAnimation>();
        if (animScript != null)
        {
          PopulateAnimationSprites(animScript, resultNum);
        }
        TempImages[j].slotImages[i].sprite = myImages[resultNum];
      }
    }

    for (int j = 0; j < numberOfSlots; j++)
    {
      bool isCaseA = int.Parse(SocketManager.ResultData.matrix[0][j]) != 0;
      float edgeRotation = isCaseA ? 50f : 10f;
      TempImages[j].slotImages[0].rectTransform.localEulerAngles = new Vector3(edgeRotation, 0, 0);
      TempImages[j].slotImages[numberOfRows - 1].rectTransform.localEulerAngles = new Vector3(-edgeRotation, 0, 0);
    }

    if (IsTurboOn || IsFreeSpin)
    {
      StopSpinToggle = true;
    }
    else
    {
      for (int i = 0; i < 5; i++)
      {
        yield return null;
        if (StopSpinToggle)
        {
          break;
        }
      }
    }

    bool willTriggerFreeSpin = SocketManager.ResultData.features.freeSpin.isFreeSpin;
    for (int i = 0; i < numberOfSlots; i++)
    {
      bool isLastReel = i == numberOfSlots - 1;
      if (willTriggerFreeSpin && isLastReel)
      {
        StartCoroutine(AnticipationZoom());
        yield return StopTweening(5, Slot_Transform[i], i, StopSpinToggle, anticipationStopDuration);
      }
      else
      {
        yield return StopTweening(5, Slot_Transform[i], i, StopSpinToggle);
      }
    }
    StopSpinToggle = false;
    yield return alltweens[^1].WaitForCompletion();

    if (Spin_Button)
    {
      Spin_Button.GetComponent<Image>().sprite = SpinSprite;
      Spin_Button.interactable = false;
    }

    KillAllTweens();

    for (int i = 0; i < numberOfRows; i++)
    {
      for (int j = 0; j < numberOfSlots; j++)
      {
        StartGameAnimation(TempImages[j].slotImages[i].gameObject);
      }
    }

    if (SocketManager.ResultData.payload.winAmount > 0)
    {
      SpinDelay = 1.2f;
    }
    else
    {
      SpinDelay = 0.2f;
    }

    if (SocketManager.ResultData.payload.winAmount > 0)
    {
      List<int> winLine = new();
      foreach (var item in SocketManager.ResultData.payload.wins)
      {
        winLine.Add(item.line);
      }
      CheckPayoutLineBackend(winLine, SocketManager.ResultData.features.jackpot.amount);
    }

    CheckPopups = true;

    if (TotalWin_text)
    {
      double displayWin = IsFreeSpin ? SocketManager.ResultData.payload.totalFreeSpinWin : SocketManager.ResultData.payload.winAmount;
      TotalWin_text.text = displayWin.ToString("F3");
    }
    BalanceTween?.Kill();
    if (Balance_text) Balance_text.text = SocketManager.ResultData.player.balance.ToString("F3");

    currentBalance = SocketManager.PlayerData.balance;

    StartCoroutine(uiManager.ShowSpinWin(SocketManager.ResultData.payload.winAmount));
    if (IsFreeSpin)
    {
      StartCoroutine(uiManager.ShowBonusWinSequence(SocketManager.ResultData.payload.winAmount, currentTotalBet));
    }

    if (SocketManager.ResultData.features.jackpot.isTriggered)
    {
      if (audioController) audioController.PlayJackpotWin();
      CheckPopups = false;
      yield return new WaitUntil(() => !CheckPopups);
      CheckPopups = true;
    }

    CheckWinPopups();

    yield return new WaitUntil(() => !CheckPopups);
    if (!IsAutoSpin && !IsFreeSpin)
    {
      ToggleButtonGrp(true);
      IsSpinning = false;
    }
    else
    {
      // yield return new WaitForSeconds(2f);
      IsSpinning = false;
    }
    if (SocketManager.ResultData.features.freeSpin.isFreeSpin)
    {
      if (IsFreeSpin)
      {
        IsFreeSpin = false;
        if (FreeSpinRoutine != null)
        {
          StopCoroutine(FreeSpinRoutine);
          FreeSpinRoutine = null;
        }
      }
      yield return StartCoroutine(uiManager.PlayFreeSpinTriggerSequence(SocketManager.ResultData.features.freeSpin.count));
      if (MiddleReelGlow) MiddleReelGlow.SetActive(true);
      yield return StartCoroutine(PlaySpecialWildReel());
      FreeSpin(SocketManager.ResultData.features.freeSpin.count);
      if (IsAutoSpin)
      {
        WasAutoSpinOn = true;
        StopAutoSpin();
        yield return new WaitForSeconds(0.1f);
      }
    }
  }
  private void BalanceDeduction()
  {
    double bet = 0;
    double balance = 0;
    try
    {
      bet = double.Parse(TotalBet_text.text);
    }
    catch (Exception e)
    {
      Debug.Log("Error while conversion " + e.Message);
    }

    try
    {
      balance = double.Parse(Balance_text.text);
    }
    catch (Exception e)
    {
      Debug.Log("Error while conversion " + e.Message);
    }
    double initAmount = balance;

    balance = balance - bet;

    BalanceTween = DOTween.To(() => initAmount, (val) => initAmount = val, balance, 0.8f).OnUpdate(() =>
    {
      if (Balance_text) Balance_text.text = initAmount.ToString("F3");
    });
  }

  internal void CheckWinPopups()
  {
    CheckPopups = false;
  }

  //generate the payout lines generated
  private void CheckPayoutLineBackend(List<int> LineId, double jackpot = 0)
  {
    if (LineId.Count > 0)
    {
      for (int i = 0; i < LineId.Count; i++)
      {
        if (PaylineGraphics.Count > LineId[i])
        {
          PaylineGraphics[LineId[i]].SetActive(true);
          StartGameAnimation(PaylineGraphics[LineId[i]]);
        }
      }

      if (jackpot > 0)
      {
        for (int i = 0; i < TempImages.Count; i++)
        {
          for (int k = 0; k < TempImages[i].slotImages.Count; k++)
          {
            StartGameAnimation(TempImages[i].slotImages[k].gameObject);
          }
        }
      }
      else
      {
        List<KeyValuePair<int, int>> coords = new();
        for (int j = 0; j < LineId.Count; j++)
        {
          for (int k = 0; k < SocketManager.ResultData.payload.wins[j].positions.Count; k++)
          {
            int rowIndex = SocketManager.InitialData.lines[LineId[j]][k];
            int columnIndex = k;
            coords.Add(new KeyValuePair<int, int>(rowIndex, columnIndex));
          }
        }

        foreach (var coord in coords)
        {
          int rowIndex = coord.Key;
          int columnIndex = coord.Value;
          StartGameAnimation(TempImages[columnIndex].slotImages[rowIndex].gameObject);
        }
      }
    }
    CheckSpinAudio = false;
  }

  #endregion

  internal void CallCloseSocket()
  {
    StartCoroutine(SocketManager.CloseSocket());
  }


  void ToggleButtonGrp(bool toggle)
  {
    bool active = toggle && !IsAutoSpin;
    if (Spin_Button) Spin_Button.interactable = toggle ? active : true;
    if (MaxBet_Button) MaxBet_Button.interactable = active;
    if (TBetMinus_Button) TBetMinus_Button.interactable = active;
    if (TBetPlus_Button) TBetPlus_Button.interactable = active;
    // if(Turbo_Button) Turbo_Button.interactable = toggle;
  }

  //start the icons animation
  private void StartGameAnimation(GameObject animObjects)
  {
    ImageAnimation temp = animObjects.GetComponent<ImageAnimation>();
    if (temp == null) return;
    temp.StartAnimation();
    TempList.Add(temp);
  }

  //stop the icons animation
  private void StopGameAnimation()
  {
    for (int i = 0; i < TempList.Count; i++)
    {
      TempList[i].StopAnimation();
    }
    TempList.Clear();
    TempList.TrimExcess();
  }


  #region TweeningCode
  private void InitializeTweening(Transform slotTransform)
  {
    slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
    Tweener tweener = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
    tweener.Play();
    alltweens.Add(tweener);
  }

  private IEnumerator PlaySpecialWildReel()
  {
    if (!SpecialReelObject || !SpecialReelTransform) yield break;
    SpecialReelObject.SetActive(true);
    SpecialReelTransform.localPosition = new Vector2(SpecialReelTransform.localPosition.x, 0);
    Tweener specialTween = SpecialReelTransform.DOLocalMoveY(-tweenHeight, 0.2f / specialReelSpeedMultiplier).SetLoops(-1, LoopType.Restart);
    yield return new WaitForSeconds(specialReelDuration);
    specialTween.Kill();
    SpecialReelObject.SetActive(false);
  }



  private IEnumerator StopTweening(int reqpos, Transform slotTransform, int index, bool isStop, float duration = 0.5f)
  {
    alltweens[index].Kill();
    // int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
    slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
    // TODO: hardcoded landing Y for now, was -tweenpos + 100 (the old IconSizeFactor-based formula). Needs tweaking and should eventually be made dynamic again instead of a fixed magic number.
    alltweens[index] = slotTransform.DOLocalMoveY(3393.2f, duration).SetEase(Ease.OutElastic);
    if (!isStop)
    {
      yield return new WaitForSeconds(0.2f);
    }
    else
    {
      yield return null;
    }
  }

  private IEnumerator AnticipationZoom()
  {
    if (!anticipationCamera) yield break;
    if (anticipationCamera.orthographic)
    {
      float original = anticipationCamera.orthographicSize;
      yield return anticipationCamera.DOOrthoSize(original * anticipationZoomAmount, anticipationZoomInDuration).SetEase(Ease.OutSine).WaitForCompletion();
      yield return anticipationCamera.DOOrthoSize(original, anticipationZoomOutDuration).SetEase(Ease.OutElastic).WaitForCompletion();
    }
    else
    {
      float original = anticipationCamera.fieldOfView;
      yield return anticipationCamera.DOFieldOfView(original * anticipationZoomAmount, anticipationZoomInDuration).SetEase(Ease.OutSine).WaitForCompletion();
      yield return anticipationCamera.DOFieldOfView(original, anticipationZoomOutDuration).SetEase(Ease.OutElastic).WaitForCompletion();
    }
  }


  private void KillAllTweens()
  {
    for (int i = 0; i < numberOfSlots; i++)
    {
      alltweens[i].Kill();
    }
    alltweens.Clear();

  }
  #endregion

}

[Serializable]
public class SlotImage
{
  public List<Image> slotImages = new List<Image>(10);
}

