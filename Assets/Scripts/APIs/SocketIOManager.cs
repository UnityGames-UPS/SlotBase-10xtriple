using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Best.SocketIO;
using Best.SocketIO.Events;
using DG.Tweening;

public class SocketIOManager : MonoBehaviour
{
  [SerializeField] private SlotBehaviour slotManager;
  [SerializeField] private UIManager uiManager;
  [SerializeField] internal JSFunctCalls JSManager;
  [SerializeField] private string testToken;
  internal GameData InitialData = null;
  internal UiData UIData = null;
  internal Features GameFeatures = null;
  internal MeterCurrentState CurrentMeterState = null;
  internal ServerResponse ResultData = null;
  internal Player PlayerData = null;
  internal bool isResultdone = false;
  internal bool isWheelBonusDone = false;
  internal bool isPickJackpotDone = false;
  internal bool SetInit = false;

  private SocketManager manager;
  protected string SocketURI = null;
  protected string TestSocketURI = "https://devrealtime.dingdinghouse.com";
  protected string nameSpace = "playground";
  private Socket gameSocket;
  protected string gameID = "SL-PT";
  private const int maxReconnectionAttempts = 6;
  private readonly TimeSpan reconnectionDelay = TimeSpan.FromSeconds(10);
  string myAuth = null;

  private bool isConnected = false;
  private bool hasEverConnected = false;

  private float lastPongTime = 0f;
  private float pingInterval = 2f;
  private float pongTimeout = 3f;
  private bool waitingForPong = false;
  private int missedPongs = 0;
  private const int MaxMissedPongs = 15;
  private Coroutine PingRoutine; //Back2 end
  private void Awake()
  {
    //Debug.unityLogger.logEnabled = false;
    SetInit = false;
  }

  private void Start()
  {
    OpenSocket();
  }

  void CloseGame()
  {
    Debug.Log("Unity: Closing Game");
    StartCoroutine(CloseSocket());  
  }

  void ReceiveAuthToken(string jsonData)
  {
    Debug.Log("Received data: " + jsonData);
    var data = JsonUtility.FromJson<AuthTokenData>(jsonData);
    SocketURI = data.socketURL;
    myAuth = data.cookie;
    nameSpace = data.nameSpace;
  }

  private void OpenSocket()
  {
    //Create and setup SocketOptions
    SocketOptions options = new SocketOptions(); //Back2 Start
    options.AutoConnect = false;
    options.Reconnection = false;
    options.Timeout = TimeSpan.FromSeconds(3); //Back2 end
    options.ConnectWith = Best.SocketIO.Transports.TransportTypes.WebSocket;

#if UNITY_WEBGL && !UNITY_EDITOR
        JSManager.SendCustomMessage("authToken");
        StartCoroutine(WaitForAuthToken(options));
#else
    object authFunction(SocketManager manager, Socket socket)
    {
      return new
      {
        token = testToken
      };
    }
    options.Auth = authFunction;
    SetupSocketManager(options);
#endif
  }

  private IEnumerator WaitForAuthToken(SocketOptions options)
  {
    // Wait until myAuth is not null
    while (myAuth == null)
    {
      Debug.Log("My Auth is null");
      yield return null;
    }
    while (SocketURI == null)
    {
      Debug.Log("My Socket is null");
      yield return null;
    }
    Debug.Log("My Auth is not null");
    // Once myAuth is set, configure the authFunction
    object authFunction(SocketManager manager, Socket socket)
    {
      return new
      {
        token = myAuth
      };
    }
    options.Auth = authFunction;
    Debug.Log("Auth function configured with token: " + myAuth);

    // Proceed with connecting to the server
    SetupSocketManager(options);

    yield return null;
  }

  private void SetupSocketManager(SocketOptions options)
  {
    Debug.Log("Setup socket manager");
    // Create and setup SocketManager
#if UNITY_EDITOR
    this.manager = new SocketManager(new Uri(TestSocketURI), options);
#else
    this.manager = new SocketManager(new Uri(SocketURI), options);
#endif

    if (string.IsNullOrEmpty(nameSpace))
    {
      gameSocket = this.manager.Socket;
    }
    else
    {
      Debug.Log("nameSpace: " + nameSpace);
      gameSocket = this.manager.GetSocket("/" + nameSpace);
    }
    // Set subscriptions
    gameSocket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
    gameSocket.On(SocketIOEventTypes.Disconnect, OnDisconnected); //Back2 Start
    gameSocket.On<Error>(SocketIOEventTypes.Error, OnError);
    gameSocket.On<string>("game:init", OnListenEvent);
    gameSocket.On<string>("result", OnResult);
    gameSocket.On<bool>("socketState", OnSocketState);
    gameSocket.On<string>("internalError", OnSocketError);
    gameSocket.On<string>("alert", OnSocketAlert);
    gameSocket.On<string>("pong", OnPongReceived); //Back2 Start
    gameSocket.On<string>("AnotherDevice", OnSocketOtherDevice);

    manager.Open(); //Back2 Start
  }

  // Connected event handler implementation
  void OnConnected(ConnectResponse resp) //Back2 Start
  {
    Debug.Log("✅ Connected to server.");

    if (hasEverConnected)
    {
      uiManager.CheckAndClosePopups();
    }

    isConnected = true;
    hasEverConnected = true;
    waitingForPong = false;
    missedPongs = 0;
    lastPongTime = Time.time;
    SendPing();
  } //Back2 end

  private void OnDisconnected() //Back2 Start
  {
    Debug.LogWarning("⚠️ Disconnected from server.");
    isConnected = false;
    ResetPingRoutine();
    uiManager.DisconnectionPopup();
  } //Back2 end

  private void OnPongReceived(string data) //Back2 Start
  {
    //Debug.Log("✅ Received pong from server.");
    waitingForPong = false;
    missedPongs = 0;
    lastPongTime = Time.time;
    //Debug.Log($"⏱️ Updated last pong time: {lastPongTime}");
    //Debug.Log($"📦 Pong payload: {data}");
  } //Back2 end

private void OnError(Error err)
  {
    Debug.LogError("Socket Error Message: " + err);
#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("error");
#endif
  }

  void OnResult(string data)
  {
    ParseResponse(data);
  }

  private void OnListenEvent(string data)
  {
    ParseResponse(data);
  }

  private void OnSocketState(bool state)
  {
    if (state)
    {
      Debug.Log("my state is " + state);
    }
  }
  private void OnSocketError(string data)
  {
    Debug.Log("Received error with data: " + data);
  }

  private void OnSocketAlert(string data)
  {
    Debug.Log("Received alert with data: " + data);
  }

  private void OnSocketOtherDevice(string data)
  {
    Debug.Log("Received Device Error with data: " + data);
    uiManager.ADfunction();
  }

  private void SendPing() //Back2 Start
  {
    ResetPingRoutine();
    PingRoutine = StartCoroutine(PingCheck());
  }

  void ResetPingRoutine()
  {
    if (PingRoutine != null)
    {
      StopCoroutine(PingRoutine);
    }
    PingRoutine = null;
  }

  private IEnumerator PingCheck()
  {
    while (true)
    {
      //Debug.Log($"🟡 PingCheck | waitingForPong: {waitingForPong}, missedPongs: {missedPongs}, timeSinceLastPong: {Time.time - lastPongTime}");

      if (missedPongs == 0)
      {
        uiManager.CheckAndClosePopups();
      }

      // If waiting for pong, and timeout passed
      if (waitingForPong)
      {
        missedPongs++;
        if (missedPongs >= 2)
          uiManager.ReconnectionPopup(missedPongs, MaxMissedPongs);
        Debug.LogWarning($"⚠️ Pong missed #{missedPongs}/{MaxMissedPongs}");

        if (missedPongs >= MaxMissedPongs)
        {
          Debug.LogError("❌ Unable to connect to server — 5 consecutive pongs missed.");
          isConnected = false;
          uiManager.DisconnectionPopup();
          yield break;
        }
      }

      // Send next ping
      waitingForPong = true;
      lastPongTime = Time.time;
      //Debug.Log("📤 Sending ping...");
      SendDataWithNamespace("ping");
      yield return new WaitForSeconds(pingInterval);
    }
  } //Back2 end
  internal void SendDataWithNamespace(string eventName, string json = null)
  {
    // Send the message
    if (gameSocket != null && gameSocket.IsOpen)
    {
      if (json != null)
      {
        gameSocket.Emit(eventName, json);
        Debug.Log("JSON data sent: " + json);
      }
      else
      {
        gameSocket.Emit(eventName);
      }
    }
    else
    {
      Debug.LogWarning("Socket is not connected.");
    }
  }

  internal IEnumerator CloseSocket() //Back2 Start
  {
    ResetPingRoutine();

    Debug.Log("Closing Socket");

    manager?.Close();
    manager = null;

    Debug.Log("Waiting for socket to close");

    yield return new WaitForSeconds(0.5f);

    Debug.Log("Socket Closed");

#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("OnExit"); //Telling the react platform user wants to quit and go back to homepage
#endif
  } //Back2 end

  private void ParseResponse(string jsonObject)
  {
    Debug.Log(jsonObject);
    ServerResponse myData = JsonConvert.DeserializeObject<ServerResponse>(jsonObject);
    if (myData == null) return;
    string id = myData.id;

    switch (id)
    {
      case "initData":
        {
          Debug.Log("Fourth Build Confirmation");
          InitialData = myData.gameData;
          UIData = myData.uiData;
          PlayerData = myData.player;
          GameFeatures = myData.features;
          CurrentMeterState = myData.features?.pinataMeters?.currentState;

          if (!SetInit)
          {
            InitialiseGame();
            SetInit = true;
          }
          break;
        }
      case "ResultData":
        {
          ResultData = myData;
          PlayerData = myData.player;
          UpdateMeterState(myData.payload?.meters);
          isResultdone = true;
          break;
        }
      case "WheelBonusResult":
        {
          ResultData = myData;
          PlayerData = myData.player;
          isWheelBonusDone = true;
          break;
        }
      case "PickJackpotResult":
        {
          ResultData = myData;
          PlayerData = myData.player;
          isPickJackpotDone = true;
          break;
        }
    }
  }

  private void UpdateMeterState(MetersUpdate meters)
  {
    if (meters == null || CurrentMeterState == null) return;
    CurrentMeterState.greenMeter = meters.greenMeter;
    CurrentMeterState.redMeter = meters.redMeter;
    CurrentMeterState.blueMeter = meters.blueMeter;
  }

  private void InitialiseGame()
  {
    slotManager.InitializeMatrix();
    slotManager.SetInitialUI();
#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("OnEnter");
#endif
  }

  internal void AccumulateResult(int currBet)
  {
    isResultdone = false;
    MessageData message = new();
    message.type = "SPIN";
    message.payload.betIndex = currBet;

    // Serialize message data to JSON
    string json = JsonUtility.ToJson(message);
    SendDataWithNamespace("request", json);
  }

  internal void SendWheelBonus()
  {
    isWheelBonusDone = false;
    MessageData message = new();
    message.type = "WHEELBONUS";
    string json = JsonUtility.ToJson(message);
    SendDataWithNamespace("request", json);
  }

  internal void SendPickJackpot()
  {
    isPickJackpotDone = false;
    MessageData message = new();
    message.type = "PICKJACKPOT";
    string json = JsonUtility.ToJson(message);
    SendDataWithNamespace("request", json);
  }

}

// ─── Emit Models ────────────────────────────────────────────────────────────
[Serializable]
public class MessageData
{
  public string type;
  public Data payload = new();
}

[Serializable]
public class Data
{
  public int betIndex;
  public string Event;
  public List<int> index;
  public int option;
}

// ─── SL-PT Server Response Models ───────────────────────────────────────────

// Top-level wrapper — handles initData, ResultData, WheelBonusResult, PickJackpotResult
[Serializable]
public class ServerResponse
{
  public string id { get; set; }
  public bool success { get; set; }
  // initData fields
  public GameData gameData { get; set; }
  public Features features { get; set; }
  public UiData uiData { get; set; }
  // ResultData fields
  public List<List<string>> matrix { get; set; }
  public SpinPayload payload { get; set; }
  // Shared
  public Player player { get; set; }
}

[Serializable]
public class GameData
{
  public List<double> bets { get; set; }
}

[Serializable]
public class Features
{
  public PinataMeters pinataMeters { get; set; }
}

[Serializable]
public class PinataMeters
{
  public bool enabled { get; set; }
  public MeterCurrentState currentState { get; set; }
}

[Serializable]
public class MeterCurrentState
{
  [JsonConverter(typeof(BoolOrIntConverter))] public int greenMeter { get; set; }
  [JsonConverter(typeof(BoolOrIntConverter))] public int redMeter { get; set; }
  [JsonConverter(typeof(BoolOrIntConverter))] public int blueMeter { get; set; }
  public int greenThreshold { get; set; }
  public int redThreshold { get; set; }
  public int blueThreshold { get; set; }
}

[Serializable]
public class SpinPayload
{
  public double winAmount { get; set; }
  public List<WaysWin> waysWins { get; set; }
  public List<CoinValue> coinWins { get; set; }
  public MetersUpdate meters { get; set; }
  public List<TriggeredFeature> triggeredFeatures { get; set; }
  public List<PendingFeature> pendingFeatures { get; set; }
  public bool isFreeSpinActive { get; set; }
  public int freeSpinsRemaining { get; set; }
  public bool isRedPinataFreeSpin { get; set; }
  public bool isBluePinataLinkBonus { get; set; }
  public bool isBluePinataFreeSpin { get; set; }
  public List<LinkBonusZone> linkBonusTargetZones { get; set; }
  public List<LockedCell> linkBonusLockedCells { get; set; }
}

[Serializable]
public class WaysWin
{
  public int symbolId { get; set; }
  public string symbolName { get; set; }
  public int matchCount { get; set; }
  public List<List<int>> positions { get; set; }
  public double payout { get; set; }
  public double basePayout { get; set; }
  public double jackpotPayout { get; set; }
}

[Serializable]
public class CoinValue
{
  public List<int> position { get; set; }
  public double value { get; set; }
}

[Serializable]
public class MetersUpdate
{
  [JsonConverter(typeof(BoolOrIntConverter))] public int greenMeter { get; set; }
  [JsonConverter(typeof(BoolOrIntConverter))] public int redMeter { get; set; }
  [JsonConverter(typeof(BoolOrIntConverter))] public int blueMeter { get; set; }
}

[Serializable]
public class TriggeredFeature
{
  public string feature { get; set; }
  public bool triggered { get; set; }
  public double awardValue { get; set; }
  // wheelBonus
  public string jackpotTier { get; set; }
  public List<string> spinHistory { get; set; }
  // pickJackpot
  public string goalJackpot { get; set; }
  // linkBonus
  public List<LockedCell> lockedCells { get; set; }
  public int respinsRemaining { get; set; }
  public bool grandTriggered { get; set; }
  public double grandBonus { get; set; }
  public double jackpotBonus { get; set; }
  public double baseWin { get; set; }
}

[Serializable]
public class PendingFeature
{
  public string feature { get; set; }
  public bool triggered { get; set; }
  public double bet { get; set; }
}

[Serializable]
public class LinkBonusZone
{
  public List<int> position { get; set; }
  public int zoneMultiplier { get; set; }
}

[Serializable]
public class LockedCell
{
  public List<int> position { get; set; }
  public int zoneMultiplier { get; set; }
  public string symbolId { get; set; }
  public double? prizeValue { get; set; }
}

// ─── Shared Models ───────────────────────────────────────────────────────────
[Serializable]
public class UiData
{
  public Paylines paylines { get; set; }
}

[Serializable]
public class Paylines
{
  public List<Symbol> symbols { get; set; }
}

[Serializable]
public class Symbol
{
  public int id { get; set; }
  public string name { get; set; }
  public List<double?> multiplier { get; set; }
  public string description { get; set; }
}

[Serializable]
public class Player
{
  public double balance { get; set; }
}

[Serializable]
public class AuthTokenData
{
  public string cookie;
  public string socketURL;
  public string nameSpace;
}

public class BoolOrIntConverter : JsonConverter<int>
{
  public override int ReadJson(JsonReader reader, Type objectType, int existingValue, bool hasExistingValue, JsonSerializer serializer)
  {
    if (reader.TokenType == JsonToken.Boolean)
      return (bool)reader.Value ? 1 : 0;
    return Convert.ToInt32(reader.Value);
  }

  public override void WriteJson(JsonWriter writer, int value, JsonSerializer serializer)
  {
    writer.WriteValue(value);
  }
}
