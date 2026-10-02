using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public const string VolumeKey = "Settings.MasterVolume";
    public const string FullscreenKey = "Settings.Fullscreen";

    [SerializeField] private string gameplayScene = "SampleScene";
    [SerializeField] private GameObject mainButtons;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Text statusText;
    private bool loading;

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        AudioListener.volume = Mathf.Clamp01(PlayerPrefs.GetFloat(VolumeKey, 1f));
        volumeSlider.SetValueWithoutNotify(AudioListener.volume);
        bool fullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;
        fullscreenToggle.SetIsOnWithoutNotify(fullscreen);
        if (PlayerPrefs.HasKey(FullscreenKey)) Screen.fullScreen = fullscreen;
        Select(playButton);
    }

    public void Play()
    {
        if (loading) return;
        if (!Application.CanStreamedLevelBeLoaded(gameplayScene))
        {
            statusText.text = "Scene game belum terdaftar di Build Profiles.";
            return;
        }
        loading = true;
        mainButtons.SetActive(false);
        statusText.text = "MEMUAT...";
        PlayerPrefs.Save();
        SceneManager.LoadSceneAsync(gameplayScene);
    }

    public void OpenSettings()
    {
        mainButtons.SetActive(false);
        settingsPanel.SetActive(true);
        Select(volumeSlider);
    }

    public void CloseSettings()
    {
        PlayerPrefs.Save();
        settingsPanel.SetActive(false);
        mainButtons.SetActive(true);
        Select(settingsButton);
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetInt(FullscreenKey, value ? 1 : 0);
    }

    public void Quit()
    {
        PlayerPrefs.Save();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private static void Select(Selectable control)
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(control.gameObject);
    }
}
