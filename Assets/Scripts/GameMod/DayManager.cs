using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Менеджер дней и событий по дням
/// </summary>
public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    [Header("Настройки")]
    [SerializeField] private int currentDay = 1;
    [SerializeField] private List<DayNewsConfig> dayNewsConfigs = new List<DayNewsConfig>();

    [Header("Катсцена пропуска дня")]
    [SerializeField] private string skipDayCutsceneId = "skip_day";
    [SerializeField] private AudioClip skipDaySound;
    [SerializeField] private AudioSource audioSource;

    // События
    public static event Action<int> OnDayChanged;
    public static event Action<int> OnDaySkipped;

    public int CurrentDay => currentDay;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Пропустить день
    /// </summary>
    public void SkipDay()
    {
        // Запускаем катсцену пропуска дня
        if (Cutscenes.CutsceneManager.Instance != null && !string.IsNullOrEmpty(skipDayCutsceneId))
        {
            Cutscenes.CutsceneManager.Instance.StartCutscene(skipDayCutsceneId);
        }

        // Стартуем звук, если есть
        if (skipDaySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(skipDaySound);
            StartCoroutine(WaitForSoundAndCompleteDaySkip());
        }
        else
        {
            // Завершаем пропуск дня сразу
            CompleteDaySkip();
        }
    }

    /// <summary>
    /// Ожидание завершения звука и завершение пропуска дня
    /// </summary>
    private IEnumerator WaitForSoundAndCompleteDaySkip()
    {
        // Ждем длительность звука
        yield return new WaitForSeconds(skipDaySound.length);

        // Завершаем пропуск дня
        CompleteDaySkip();
    }

    /// <summary>
    /// Завершить пропуск дня
    /// </summary>
    private void CompleteDaySkip()
    {
        int previousDay = currentDay;
        currentDay++;

        Debug.Log($"[DayManager] День пропущен: {previousDay} -> {currentDay}");

        OnDaySkipped?.Invoke(currentDay);
        OnDayChanged?.Invoke(currentDay);

        // Завершаем катсцену
        if (Cutscenes.CutsceneManager.Instance != null)
        {
            Cutscenes.CutsceneManager.Instance.EndCutscene();
        }
    }

    /// <summary>
    /// Получить ID диалога с новостями для текущего дня
    /// </summary>
    public string GetNewsDialogIdForCurrentDay()
    {
        return GetNewsDialogIdForDay(currentDay);
    }

    /// <summary>
    /// Получить ID диалога с новостями для конкретного дня
    /// </summary>
    public string GetNewsDialogIdForDay(int day)
    {
        var config = dayNewsConfigs.Find(c => c.day == day);
        return config?.newsDialogId;
    }

    /// <summary>
    /// Проверить, есть ли новости для текущего дня
    /// </summary>
    public bool HasNewsForCurrentDay()
    {
        return !string.IsNullOrEmpty(GetNewsDialogIdForCurrentDay());
    }

    /// <summary>
    /// Проверить, есть ли новости для конкретного дня
    /// </summary>
    public bool HasNewsForDay(int day)
    {
        return !string.IsNullOrEmpty(GetNewsDialogIdForDay(day));
    }

    /// <summary>
    /// Установить текущий день (для отладки или загрузки сохранения)
    /// </summary>
    public void SetCurrentDay(int day)
    {
        if (day < 1)
        {
            Debug.LogWarning($"[DayManager] Попытка установить некорректный день: {day}. Минимум 1.");
            return;
        }

        int previousDay = currentDay;
        currentDay = day;

        Debug.Log($"[DayManager] День установлен: {previousDay} -> {currentDay}");

        OnDayChanged?.Invoke(currentDay);
    }
}

/// <summary>
/// Конфигурация новостей для дня
/// </summary>
[System.Serializable]
public class DayNewsConfig
{
    [Tooltip("День")]
    public int day = 1;

    [Tooltip("ID диалога с новостями для этого дня")]
    public string newsDialogId;
}