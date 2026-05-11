using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Interfaz del menú principal.
 * Gestiona audio, volumen, navegación y carga de preferencias.
 */
public class MainMenuUI : MonoBehaviour
{
    public GameObject logo;
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject statsPanel;

    [Header("Menu Buttons")]
    public Button[] mainMenuButtons;
    public Selectable[] settingsItems;
    public Selectable[] statsItems;

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

    // Tiempo entre movimientos del stick
    private float nextInputTime;

    // Delay para evitar spam del mando
    [SerializeField] private float inputDelay = 0.2f;

    public PreferencesManager preferencesManager;

    private void Start()
    {
        PreferencesData data = GetPreferencesManager().Load();

        bgmEnabled = data.music;
        sfxEnabled = data.sfx;
        volumeLevel = data.volume;
        if (logo == null)
        {
            logo = GameObject.Find("Logo");
        }

        UpdateAudioTexts();
        UpdateVolumeBar();
        ShowMainMenu();
    }

    private void Update()
    {
        HandleInput();
    }

    // Maneja teclado y mando.
    private void HandleInput()
    {
        // NAVEGACIÓN VERTICAL
        float vertical = Input.GetAxisRaw("Vertical");

        bool moveDown =
            Input.GetKeyDown(KeyCode.S) ||
            (vertical < -0.5f && Time.unscaledTime > nextInputTime);

        bool moveUp =
            Input.GetKeyDown(KeyCode.W) ||
            (vertical > 0.5f && Time.unscaledTime > nextInputTime);

        if (moveDown)
        {
            nextInputTime = Time.unscaledTime + inputDelay;
            Move(1);
        }
        else if (moveUp)
        {
            nextInputTime = Time.unscaledTime + inputDelay;
            Move(-1);
        }

        // CONFIRMAR BOTÓN
        if (
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.JoystickButton0)
        )
        {
            // Evita activar el botón de volumen
            if (inSettings && currentIndex == volumeIndex)
                return;

            Button btn = currentMenu[currentIndex] as Button;

            if (btn != null)
                btn.onClick.Invoke();
        }

        // CONTROL DE VOLUMEN
        if (
            inSettings &&
            currentMenu[currentIndex] != null &&
            currentMenu[currentIndex].gameObject == volumeButton
        )
        {
            float horizontal = Input.GetAxisRaw("Horizontal");

            bool increase =
                Input.GetKeyDown(KeyCode.D) ||
                (horizontal > 0.5f && Time.unscaledTime > nextInputTime);

            bool decrease =
                Input.GetKeyDown(KeyCode.A) ||
                (horizontal < -0.5f && Time.unscaledTime > nextInputTime);

            if (increase)
            {
                nextInputTime = Time.unscaledTime + inputDelay;
                IncreaseVolume();
            }

            if (decrease)
            {
                nextInputTime = Time.unscaledTime + inputDelay;
                DecreaseVolume();
            }
        }

        // VOLVER ATRÁS

        if (
            (
                Input.GetKeyDown(KeyCode.Escape) ||
                Input.GetKeyDown(KeyCode.JoystickButton1)
            ) &&
            inSettings
        )
        {
            BackToMain();
        }
    }

    // Mueve el foco por el menú.
    private void Move(int direction)
    {
        if (currentMenu == null || currentMenu.Length == 0)
            return;

        currentIndex += direction;

        if (currentIndex < 0)
            currentIndex = currentMenu.Length - 1;
        else if (currentIndex >= currentMenu.Length)
            currentIndex = 0;

        Highlight();
    }

    // Resalta el elemento actual
    private void Highlight()
    {
        if (currentMenu == null || currentMenu.Length == 0)
            return;

        if (EventSystem.current == null)
            return;

        if (currentMenu[currentIndex] == null)
            return;

        EventSystem.current.SetSelectedGameObject(
            currentMenu[currentIndex].gameObject
        );
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
        statsPanel.SetActive(false);

        SetMenu(mainMenuButtons);
    }

    public void OpenSettings()
    {
        inSettings = true;

        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);

        SetMenu(settingsItems);
    }

    public void OpenStats()
    {
        inSettings = true;

        mainMenuPanel.SetActive(false);
        statsPanel.SetActive(true);
        logo.SetActive(false);

        SetMenu(statsItems);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void PlayTestLevel()
    {
        SceneManager.LoadScene("Level_Test");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void BackToMain()
    {
        logo.SetActive(true);
        ShowMainMenu();
    }

    // AUDIO

    public void ToggleBGM()
    {
        bgmEnabled = !bgmEnabled;

        AudioManager.instance.SetBGM(bgmEnabled);
        UpdateAudioTexts();

        SavePreferences();
    }

    public void ToggleSFX()
    {
        sfxEnabled = !sfxEnabled;

        AudioManager.instance.SetSFX(sfxEnabled);
        UpdateAudioTexts();

        SavePreferences();
    }

    private void UpdateAudioTexts()
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
        AudioManager.instance.SetVolume(normalized);

        SavePreferences();
    }

    private void UpdateVolumeBar()
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

    // Guarda el estado actual en el JSON y lo sube a MySQL. Se hace una sola vez para evitar sincronizaciones duplicadas
    private void SavePreferences()
    {
        PreferencesData data = new PreferencesData
        {
            music = bgmEnabled,
            sfx = sfxEnabled,
            volume = volumeLevel
        };

        GetPreferencesManager().Save(data);
        GetPreferencesManager().ImportDatabase();
    }

    // Devuelve el gestor de preferencias. Primero intenta usar la referencia asignada en el inspector y después cae al singleton si hace falta
    private PreferencesManager GetPreferencesManager()
    {
        if (preferencesManager != null)
            return preferencesManager;

        return PreferencesManager.Instance;
    }
}