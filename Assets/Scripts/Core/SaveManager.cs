using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    // Keys
    private const string KEY_HIGH_SCORE = "HighScore";
    private const string KEY_TOTAL_GEMS = "TotalGems";
    private const string KEY_TUTORIAL_DONE = "TutorialDone";
    private const string KEY_EQUIPPED_SKIN = "EquippedSkin";
    private const string KEY_UNLOCKED_SKINS = "UnlockedSkins";
    private const string KEY_ACHIEVEMENTS = "Achievements";
    private const string KEY_DAILY_LOGIN = "DailyLogin";
    private const string KEY_SFX_ON = "SFXOn";
    private const string KEY_MUSIC_ON = "MusicOn";
    private const string KEY_VIBRATION_ON = "VibrationOn";
    private const string KEY_LANGUAGE = "Language";

    // Cached values
    public int HighScore { get; private set; }
    public int TotalGems { get; private set; }
    public bool TutorialDone { get; private set; }
    public string EquippedSkin { get; private set; }
    public string UnlockedSkins { get; private set; }
    public string Achievements { get; private set; }
    public string DailyLogin { get; private set; }
    public bool SFXOn { get; private set; }
    public bool MusicOn { get; private set; }
    public bool VibrationOn { get; private set; }
    public string Language { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Load();
    }

    void Load()
    {
        HighScore = PlayerPrefs.GetInt(KEY_HIGH_SCORE, 0);
        TotalGems = PlayerPrefs.GetInt(KEY_TOTAL_GEMS, 0);
        TutorialDone = PlayerPrefs.GetInt(KEY_TUTORIAL_DONE, 0) == 1;
        EquippedSkin = PlayerPrefs.GetString(KEY_EQUIPPED_SKIN, "Default");
        UnlockedSkins = PlayerPrefs.GetString(KEY_UNLOCKED_SKINS, "[\"Default\"]");
        Achievements = PlayerPrefs.GetString(KEY_ACHIEVEMENTS, "{}");
        DailyLogin = PlayerPrefs.GetString(KEY_DAILY_LOGIN, "{}");
        SFXOn = PlayerPrefs.GetInt(KEY_SFX_ON, 1) == 1;
        MusicOn = PlayerPrefs.GetInt(KEY_MUSIC_ON, 1) == 1;
        VibrationOn = PlayerPrefs.GetInt(KEY_VIBRATION_ON, 1) == 1;
        Language = PlayerPrefs.GetString(KEY_LANGUAGE, "FR");
    }

    // --- High Score ---
    public bool TrySetHighScore(int wave)
    {
        if (wave <= HighScore) return false;
        HighScore = wave;
        PlayerPrefs.SetInt(KEY_HIGH_SCORE, HighScore);
        PlayerPrefs.Save();
        return true;
    }

    // --- Gems (permanent) ---
    public void AddGems(int amount)
    {
        TotalGems += amount;
        PlayerPrefs.SetInt(KEY_TOTAL_GEMS, TotalGems);
        PlayerPrefs.Save();
    }

    public bool SpendGems(int amount)
    {
        if (TotalGems < amount) return false;
        TotalGems -= amount;
        PlayerPrefs.SetInt(KEY_TOTAL_GEMS, TotalGems);
        PlayerPrefs.Save();
        return true;
    }

    // --- Tutorial ---
    public void SetTutorialDone()
    {
        TutorialDone = true;
        PlayerPrefs.SetInt(KEY_TUTORIAL_DONE, 1);
        PlayerPrefs.Save();
    }

    // --- Skins ---
    public void SetEquippedSkin(string skinId)
    {
        EquippedSkin = skinId;
        PlayerPrefs.SetString(KEY_EQUIPPED_SKIN, skinId);
        PlayerPrefs.Save();
    }

    public void SetUnlockedSkins(string json)
    {
        UnlockedSkins = json;
        PlayerPrefs.SetString(KEY_UNLOCKED_SKINS, json);
        PlayerPrefs.Save();
    }

    // --- Achievements ---
    public void SetAchievements(string json)
    {
        Achievements = json;
        PlayerPrefs.SetString(KEY_ACHIEVEMENTS, json);
        PlayerPrefs.Save();
    }

    // --- Daily Login ---
    public void SetDailyLogin(string json)
    {
        DailyLogin = json;
        PlayerPrefs.SetString(KEY_DAILY_LOGIN, json);
        PlayerPrefs.Save();
    }

    // --- Settings ---
    public void SetSFX(bool on) { SFXOn = on; PlayerPrefs.SetInt(KEY_SFX_ON, on ? 1 : 0); PlayerPrefs.Save(); }
    public void SetMusic(bool on) { MusicOn = on; PlayerPrefs.SetInt(KEY_MUSIC_ON, on ? 1 : 0); PlayerPrefs.Save(); }
    public void SetVibration(bool on) { VibrationOn = on; PlayerPrefs.SetInt(KEY_VIBRATION_ON, on ? 1 : 0); PlayerPrefs.Save(); }
    public void SetLanguage(string lang) { Language = lang; PlayerPrefs.SetString(KEY_LANGUAGE, lang); PlayerPrefs.Save(); }

    // --- Debug ---
    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        Load();
        Debug.Log("SaveManager: all data reset");
    }
}
