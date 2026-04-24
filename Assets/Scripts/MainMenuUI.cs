using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Menu Buttons")]
    public Button[] mainMenuButtons;
    public Selectable[] settingsItems;

    [Header("Settings - Audio")]
    public bool bgmEnabled = true;
    public bool sfxEnabled = true;

    public TextMeshProUGUI bgmText;
    public TextMeshProUGUI sfxText;

    [Header("Volume")]
    public GameObject volumeButton;
    public GameObject[] volumeBlocks;
    [Range(0, 10)]
    public int volumeLevel = 10;
    public int volumeIndex = 2;

    private Selectable[] currentMenu;
    private int currentIndex;
    private bool inSettings;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        UpdateVolumeBar();
        UpdateAudioTexts();

        ShowMainMenu();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.S))
            Move(1);
        else if (Input.GetKeyDown(KeyCode.W))
            Move(-1);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (inSettings && currentIndex == volumeIndex)
                return;

            var btn = currentMenu[currentIndex] as Button;
            if (btn != null)
                btn.onClick.Invoke();
        }

        if (inSettings && currentMenu[currentIndex].gameObject == volumeButton)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                IncreaseVolume();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                DecreaseVolume();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && inSettings)
            BackToMain();
    }

    void Move(int direction)
    {
        currentIndex += direction;

        if (currentIndex < 0)
            currentIndex = currentMenu.Length - 1;
        else if (currentIndex >= currentMenu.Length)
            currentIndex = 0;

        Highlight();
    }

    void Highlight()
    {
        EventSystem.current.SetSelectedGameObject(currentMenu[currentIndex].gameObject);
    }

    public void SetMenu(Selectable[] menu)
    {
        currentMenu = menu;
        currentIndex = 0;
        Highlight();
    }

    // PANEL MANAGEMENT
    public void ShowMainMenu()
    {
        inSettings = false;

        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);

        SetMenu(mainMenuButtons);
    }

    public void OpenSettings()
    {
        inSettings = true;

        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);

        SetMenu(settingsItems);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void BackToMain()
    {
        ShowMainMenu();
    }

    // AUDIO
    public void ToggleBGM()
    {
        bgmEnabled = !bgmEnabled;

        AudioManager.instance.SetBGM(bgmEnabled);
        UpdateAudioTexts();
    }

    public void ToggleSFX()
    {
        sfxEnabled = !sfxEnabled;

        AudioManager.instance.SetSFX(sfxEnabled);
        UpdateAudioTexts();
    }

    void UpdateAudioTexts()
    {
        if (bgmText != null)
            bgmText.text = bgmEnabled ? "BGM    ON" : "BGM  OFF";

        if (sfxText != null)
            sfxText.text = sfxEnabled ? "SFX    ON" : "SFX  OFF";
    }

    // VOLUME
    public void SetVolume(int value)
    {
        volumeLevel = Mathf.Clamp(value, 0, 10);

        UpdateVolumeBar();

        float normalized = volumeLevel / 10f;
        normalized = Mathf.Clamp01(normalized);

        if (AudioManager.instance != null)
            AudioManager.instance.SetVolume(normalized);
    }

    void UpdateVolumeBar()
    {
        for (int i = 0; i < volumeBlocks.Length; i++)
        {
            volumeBlocks[i].SetActive(i < volumeLevel);
        }
    }

    public void IncreaseVolume()
    {
        SetVolume(volumeLevel + 1);
    }

    public void DecreaseVolume()
    {
        SetVolume(volumeLevel - 1);
    }
}