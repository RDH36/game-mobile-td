using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("SFX Clips")]
    [SerializeField] private AudioClip cannonFire;
    [SerializeField] private AudioClip hitWall;
    [SerializeField] private AudioClip monsterHit;
    [SerializeField] private AudioClip cannonDamage;
    [SerializeField] private AudioClip coinCollect;
    [SerializeField] private AudioClip uiClick;

    [Header("Music Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip actionMusic;
    [SerializeField] private AudioClip bossMusic;

    [Header("SFX Settings")]
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 0.7f;
    [SerializeField] private float pitchMin = 0.9f;
    [SerializeField] private float pitchMax = 1.1f;

    [Header("Music Settings")]
    [SerializeField][Range(0f, 1f)] private float musicVolume = 0.3f;
    [SerializeField] private float crossfadeDuration = 1f;

    private AudioSource _sfxSource;
    private AudioSource _musicSourceA;
    private AudioSource _musicSourceB;
    private bool _usingSourceA = true;
    private Coroutine _crossfadeRoutine;
    private bool _isBossMusic;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;

        _musicSourceA = gameObject.AddComponent<AudioSource>();
        _musicSourceA.loop = true;
        _musicSourceA.playOnAwake = false;
        _musicSourceA.volume = 0f;

        _musicSourceB = gameObject.AddComponent<AudioSource>();
        _musicSourceB.loop = true;
        _musicSourceB.playOnAwake = false;
        _musicSourceB.volume = 0f;
    }

    void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;

        var waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
            waveManager.OnBossWaveStarted += HandleBossWaveStarted;

        // Start with menu music
        PlayMusic(menuMusic);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    void HandleGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                _isBossMusic = false;
                PlayMusic(menuMusic);
                break;
            case GameState.Playing:
                if (!_isBossMusic)
                    PlayMusic(actionMusic);
                break;
            case GameState.WaveComplete:
                if (_isBossMusic)
                {
                    _isBossMusic = false;
                    PlayMusic(actionMusic);
                }
                break;
            case GameState.GameOver:
            case GameState.Victory:
                _isBossMusic = false;
                PlayMusic(menuMusic);
                break;
        }
    }

    void HandleBossWaveStarted(BossData bossData)
    {
        _isBossMusic = true;
        if (bossMusic != null)
            PlayMusic(bossMusic);
    }

    // --- SFX ---

    private void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || _sfxSource == null) return;
        _sfxSource.pitch = Random.Range(pitchMin, pitchMax);
        _sfxSource.PlayOneShot(clip, sfxVolume * volumeScale);
    }

    public void PlayCannonFire() => PlaySFX(cannonFire);
    public void PlayHitWall() => PlaySFX(hitWall, 0.5f);
    public void PlayMonsterHit() => PlaySFX(monsterHit);
    public void PlayCannonDamage() => PlaySFX(cannonDamage);
    public void PlayCoinCollect() => PlaySFX(coinCollect, 0.8f);
    public void PlayUIClick() => PlaySFX(uiClick, 1.5f);

    // --- Music ---

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        var incoming = _usingSourceA ? _musicSourceA : _musicSourceB;
        var outgoing = _usingSourceA ? _musicSourceB : _musicSourceA;

        // Skip if already playing the same clip
        if (incoming.clip == clip && incoming.isPlaying) return;

        incoming.clip = clip;
        incoming.Play();

        if (_crossfadeRoutine != null)
            StopCoroutine(_crossfadeRoutine);
        _crossfadeRoutine = StartCoroutine(Crossfade(incoming, outgoing));

        _usingSourceA = !_usingSourceA;
    }

    private System.Collections.IEnumerator Crossfade(AudioSource incoming, AudioSource outgoing)
    {
        float t = 0f;
        float outStartVol = outgoing.volume;

        while (t < crossfadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = t / crossfadeDuration;
            incoming.volume = Mathf.Lerp(0f, musicVolume, p);
            outgoing.volume = Mathf.Lerp(outStartVol, 0f, p);
            yield return null;
        }

        incoming.volume = musicVolume;
        outgoing.volume = 0f;
        outgoing.Stop();
        _crossfadeRoutine = null;
    }
}
