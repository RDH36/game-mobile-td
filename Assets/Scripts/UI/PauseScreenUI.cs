using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

public class PauseScreenUI : MonoBehaviour
{
    private GameObject _panel;
    private Button _resumeButton;
    private Button _quitButton;

    private bool _isPaused;

    void Awake()
    {
        FindUIElements();
        if (_panel != null) _panel.SetActive(false);
    }

    void Start()
    {
        if (_resumeButton != null)
            _resumeButton.onClick.AddListener(Resume);
        if (_quitButton != null)
            _quitButton.onClick.AddListener(Quit);
    }

    void FindUIElements()
    {
        Transform canvas = transform;
        Transform panelT = canvas.Find("PausePanel");
        if (panelT == null) return;

        _panel = panelT.gameObject;
        Transform resumeT = panelT.Find("ResumeButton");
        if (resumeT != null) _resumeButton = resumeT.GetComponent<Button>();
        Transform quitT = panelT.Find("QuitButton");
        if (quitT != null) _quitButton = quitT.GetComponent<Button>();
    }

    public void TogglePause()
    {
        if (_isPaused)
            Resume();
        else
            Pause();
    }

    void Pause()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        _isPaused = true;
        Time.timeScale = 0f;
        if (_panel != null)
        {
            _panel.SetActive(true);
            SpringInChildren(_panel.transform);
        }
    }

    void Resume()
    {
        SFXManager.Instance?.PlayUIClick();
        _isPaused = false;
        Time.timeScale = 1f;
        if (_panel != null) _panel.SetActive(false);
    }

    void SpringInChildren(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            var spring = child.GetComponent<MMSpringScale>();
            if (spring == null)
            {
                spring = child.gameObject.AddComponent<MMSpringScale>();
                spring.TimeScaleMode = MMSpringComponentBase.TimeScaleModes.Unscaled;
            }
            spring.MoveToInstant(Vector3.zero);
            spring.MoveTo(Vector3.one);
        }
    }

    void Quit()
    {
        SFXManager.Instance?.PlayUIClick();
        Resume();
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
    }
}
