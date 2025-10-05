using UnityEngine;
using System.Collections.Generic;

public enum GameMode
{
    Play,
    Dialogue,
    Cutscene
}

[System.Serializable]
public class ModeSettings
{
    public GameMode mode;
    public List<GameObject> objectsToEnable = new List<GameObject>();
    public List<GameObject> objectsToDisable = new List<GameObject>();
}

public class GameModeManager : MonoBehaviour
{
    [Header("Режимы игры")] [SerializeField]
    private List<ModeSettings> modeSettingsList = new List<ModeSettings>();

    [Header("Текущий режим")] [SerializeField]
    private GameMode _currentMode = GameMode.Play;

    public GameMode CurrentMode
    {
        get => _currentMode;
        set
        {
            _currentMode = value;
            ApplyModeSettings(_currentMode);
        }
    }


    void Start()
    {
        // Применяем начальный режим
        ApplyModeSettings(_currentMode);
    }

    void OnValidate()
    {
        // Применяем режим при изменении в инспекторе (в режиме редактора и во время игры)
        if (Application.isPlaying)
        {
            CurrentMode = _currentMode;
        }
    }

    /// <summary>
    /// Переключить режим игры
    /// </summary>
    public void SwitchMode(GameMode newMode)
    {
        CurrentMode = newMode;
    }

    /// <summary>
    /// Применить настройки для текущего режима
    /// </summary>
    private void ApplyModeSettings(GameMode mode)
    {
        ModeSettings settings = modeSettingsList.Find(s => s.mode == mode);

        if (settings == null)
        {
            Debug.LogWarning($"Настройки для режима {mode} не найдены!");
            return;
        }

        // Включаем объекты для этого режима
        foreach (GameObject obj in settings.objectsToEnable)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        // Отключаем объекты для этого режима
        foreach (GameObject obj in settings.objectsToDisable)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}