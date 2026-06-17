using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private const string PrefKeyMusic = "audio_music_enabled";
    private const string PrefKeySFX = "audio_sfx_enabled";

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgMusicSource;
    [SerializeField] private AudioSource bonusBgSource;
    [SerializeField] private AudioSource spinSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource overlapSource;

    [Header("BG Music")]
    [SerializeField] private AudioClip clipBg;
    [SerializeField] private AudioClip clipBonusBg;

    [Header("UI")]
    [SerializeField] private AudioClip clipButton;
    [SerializeField] private AudioClip clipUIClick;
    [SerializeField] private AudioClip clipGameStarted;

    [Header("Spin")]
    [SerializeField] private AudioClip clipSpinLoop;
    [SerializeField] private AudioClip clipReelStop;

    [Header("Symbols")]
    [SerializeField] private AudioClip clipNormalIcon;
    [SerializeField] private AudioClip clipJackpotIcon;
    [SerializeField] private AudioClip clipBubblePop;
    [SerializeField] private AudioClip clipCoinValueAppear;

    [Header("Features")]
    [SerializeField] private AudioClip clipBatHit;
    [SerializeField] private AudioClip clipFreeGameStarted;
    [SerializeField] private AudioClip clipFreeGameBonus;
    [SerializeField] private AudioClip clipBigBonus;
    [SerializeField] private AudioClip clipInsideJackpot;
    [SerializeField] private AudioClip clipChoosedJackpot;
    [SerializeField] private AudioClip clipTimerClock;

    [Header("Jackpots")]
    [SerializeField] private AudioClip clipMiniJackpot;
    [SerializeField] private AudioClip clipMinorJackpot;
    [SerializeField] private AudioClip clipMajorJackpot;
    [SerializeField] private AudioClip clipMegaJackpot;
    [SerializeField] private AudioClip clipGrandJackpot;

    [Header("Win")]
    [SerializeField] private AudioClip clipBigWin;

    private bool _musicEnabled = true;
    private bool _sfxEnabled = true;

    internal bool MusicEnabled => _musicEnabled;
    internal bool SfxEnabled => _sfxEnabled;

    private void Awake()
    {
        _musicEnabled = PlayerPrefs.GetInt(PrefKeyMusic, 1) == 1;
        _sfxEnabled = PlayerPrefs.GetInt(PrefKeySFX, 1) == 1;
        ApplyMusicVolume();
        ApplySfxVolume();
    }

    private void Start()
    {
        PlayBgMusic();
    }

    // ── Volume Control ────────────────────────────────────────────────────────

    internal void SetMusicEnabled(bool on)
    {
        _musicEnabled = on;
        PlayerPrefs.SetInt(PrefKeyMusic, on ? 1 : 0);
        PlayerPrefs.Save();
        ApplyMusicVolume();
    }

    internal void SetSfxEnabled(bool on)
    {
        _sfxEnabled = on;
        PlayerPrefs.SetInt(PrefKeySFX, on ? 1 : 0);
        PlayerPrefs.Save();
        ApplySfxVolume();
    }

    private void ApplyMusicVolume()
    {
        if (bgMusicSource) bgMusicSource.volume = _musicEnabled ? 1f : 0f;
        if (bonusBgSource) bonusBgSource.volume = _musicEnabled ? 1f : 0f;
    }

    private void ApplySfxVolume()
    {
        float v = _sfxEnabled ? 1f : 0f;
        if (sfxSource) sfxSource.volume = v;
        if (overlapSource) overlapSource.volume = v;
        if (spinSource) spinSource.volume = v;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void PlayOneShot(AudioSource source, AudioClip clip)
    {
        if (!_sfxEnabled || source == null || clip == null) return;
        source.PlayOneShot(clip);
    }

    private void PlayLoop(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null) return;
        source.clip = clip;
        source.loop = true;
        source.Play();
    }

    private void StopSource(AudioSource source)
    {
        if (source == null) return;
        source.Stop();
        source.loop = false;
    }

    // ── BG Music ──────────────────────────────────────────────────────────────

    internal void PlayBgMusic()
    {
        if (bgMusicSource == null || clipBg == null) return;
        if (bgMusicSource.isPlaying && bgMusicSource.clip == clipBg) return;
        bgMusicSource.clip = clipBg;
        bgMusicSource.loop = true;
        bgMusicSource.volume = _musicEnabled ? 1f : 0f;
        bgMusicSource.Play();
    }

    internal void PlayBonusBgMusic()
    {
        if (bonusBgSource == null || clipBonusBg == null) return;
        StopSource(bgMusicSource);
        bonusBgSource.clip = clipBonusBg;
        bonusBgSource.loop = true;
        bonusBgSource.volume = _musicEnabled ? 1f : 0f;
        bonusBgSource.Play();
    }

    internal void StopBonusBgMusic()
    {
        StopSource(bonusBgSource);
        PlayBgMusic();
    }

    // ── UI ────────────────────────────────────────────────────────────────────

    internal void PlayButton() => PlayOneShot(sfxSource, clipButton);
    internal void PlayUIClick() => PlayOneShot(sfxSource, clipUIClick);
    internal void PlayGameStarted() => PlayOneShot(sfxSource, clipGameStarted);

    // ── Spin ──────────────────────────────────────────────────────────────────

    internal void PlaySpinLoop()
    {
        if (spinSource == null || clipSpinLoop == null) return;
        spinSource.clip = clipSpinLoop;
        spinSource.loop = true;
        spinSource.volume = _sfxEnabled ? 1f : 0f;
        spinSource.Play();
    }

    internal void StopSpinLoop() => StopSource(spinSource);

    internal void PlayReelStop() => PlayOneShot(overlapSource, clipReelStop);

    // ── Symbols ───────────────────────────────────────────────────────────────

    internal void PlayNormalIcon() => PlayOneShot(sfxSource, clipNormalIcon);
    internal void PlayJackpotIcon() => PlayOneShot(sfxSource, clipJackpotIcon);
    internal void PlayBubblePop() => PlayOneShot(overlapSource, clipBubblePop);
    internal void PlayCoinValueAppear() => PlayOneShot(overlapSource, clipCoinValueAppear);

    // ── Features ──────────────────────────────────────────────────────────────

    internal void PlayBatHit() => PlayOneShot(sfxSource, clipBatHit);
    internal void PlayFreeGameStarted() => PlayOneShot(sfxSource, clipFreeGameStarted);
    internal void PlayFreeGameBonus() => PlayOneShot(sfxSource, clipFreeGameBonus);
    internal void PlayBigBonus() => PlayOneShot(sfxSource, clipBigBonus);
    internal void PlayInsideJackpot() => PlayOneShot(sfxSource, clipInsideJackpot);
    internal void PlayChoosedJackpot() => PlayOneShot(sfxSource, clipChoosedJackpot);

    internal void PlayTimerClock()
    {
        if (sfxSource == null || clipTimerClock == null) return;
        sfxSource.clip = clipTimerClock;
        sfxSource.loop = true;
        sfxSource.volume = _sfxEnabled ? 1f : 0f;
        sfxSource.Play();
    }

    internal void StopTimerClock() => StopSource(sfxSource);

    // ── Jackpots ──────────────────────────────────────────────────────────────

    internal void PlayMiniJackpot() => PlayOneShot(sfxSource, clipMiniJackpot);
    internal void PlayMinorJackpot() => PlayOneShot(sfxSource, clipMinorJackpot);
    internal void PlayMajorJackpot() => PlayOneShot(sfxSource, clipMajorJackpot);
    internal void PlayMegaJackpot() => PlayOneShot(sfxSource, clipMegaJackpot);
    internal void PlayGrandJackpot() => PlayOneShot(sfxSource, clipGrandJackpot);

    // ── Win ───────────────────────────────────────────────────────────────────

    internal void PlayBigWin() => PlayOneShot(sfxSource, clipBigWin);

    // ── Focus Handling ────────────────────────────────────────────────────────

    private void OnApplicationFocus(bool hasFocus)
    {
        HandleFocus(hasFocus);
    }

    private void OnApplicationPause(bool isPaused)
    {
        HandleFocus(!isPaused);
    }

    private void HandleFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            AudioListener.volume = 0f;
        }
        else
        {
            AudioListener.volume = 1f;
        }
    }
}
