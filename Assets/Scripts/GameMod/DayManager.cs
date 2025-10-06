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

    [Header("Система энергии")]
    [SerializeField] private int maxEnergy = 1;
    [SerializeField] private int currentEnergy = 1;
    [SerializeField] private int minEnergyToAct = 1;
    [SerializeField] private string tiredDialogId = "need_to_sleep";
    

    public int CurrentDay => currentDay;
    public int CurrentEnergy => currentEnergy;
    public int MaxEnergy => maxEnergy;

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
        StartCoroutine(SkipDayCoroutine());
    }

    /// <summary>
    /// Корутина пропуска дня с задержкой
    /// </summary>
    private IEnumerator SkipDayCoroutine()
    {
        // Ждем 100 мс перед выполнением
        yield return new WaitForSeconds(0.1f);

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

        // Восстанавливаем энергию после сна
        RestoreEnergy();

        Debug.Log($"[DayManager] День пропущен: {previousDay} -> {currentDay}");
        

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
    }

    #region Система энергии

    /// <summary>
    /// Восстановить энергию до максимума (после сна)
    /// </summary>
    public void RestoreEnergy()
    {
        currentEnergy = maxEnergy;
        Debug.Log($"[DayManager] Энергия восстановлена: {currentEnergy}/{maxEnergy}");
    }

    /// <summary>
    /// Потратить энергию
    /// </summary>
    /// <param name="amount">Количество энергии для траты</param>
    /// <returns>True если энергия была потрачена, false если недостаточно энергии</returns>
    public bool SpendEnergy(int amount)
    {
        if (currentEnergy - amount < 0)
        {
            Debug.LogWarning($"[DayManager] Недостаточно энергии! Требуется: {amount}, доступно: {currentEnergy}");
            return false;
        }

        currentEnergy -= amount;
        Debug.Log($"[DayManager] Потрачено энергии: {amount}. Осталось: {currentEnergy}/{maxEnergy}");

        return true;
    }

    /// <summary>
    /// Добавить энергию (например, после отдыха или использования предмета)
    /// </summary>
    /// <param name="amount">Количество энергии для добавления</param>
    public void AddEnergy(int amount)
    {
        currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
        Debug.Log($"[DayManager] Добавлено энергии: {amount}. Текущая энергия: {currentEnergy}/{maxEnergy}");
    }

    /// <summary>
    /// Проверить, достаточно ли энергии для действия
    /// </summary>
    /// <returns>True если энергии достаточно</returns>
    public bool HasEnoughEnergy()
    {
        return currentEnergy >= minEnergyToAct;
    }

    /// <summary>
    /// Проверить, достаточно ли энергии для действия с определенной стоимостью
    /// </summary>
    /// <param name="amount">Необходимое количество энергии</param>
    /// <returns>True если энергии достаточно</returns>
    public bool HasEnoughEnergy(int amount)
    {
        return currentEnergy >= amount;
    }

    /// <summary>
    /// Показать диалог о том, что игрок устал и нужно поспать
    /// </summary>
    public void ShowTiredDialog()
    {
        if (!string.IsNullOrEmpty(tiredDialogId))
        {
            Debug.Log($"[DayManager] Игрок устал! Энергия: {currentEnergy}/{maxEnergy}");

            if (Dialogs.DialogManager.Instance != null)
            {
                Dialogs.DialogManager.Instance.StartDialog(tiredDialogId);
            }
            else
            {
                Debug.LogWarning($"[DayManager] DialogManager не найден! Не могу показать диалог '{tiredDialogId}'");
            }
        }
        else
        {
            Debug.LogWarning("[DayManager] ID диалога усталости не задан!");
        }
    }

    /// <summary>
    /// Попытаться выполнить действие, требующее энергии
    /// Если энергии недостаточно, показывает диалог усталости
    /// </summary>
    /// <param name="energyCost">Стоимость действия в энергии</param>
    /// <returns>True если действие можно выполнить</returns>
    public bool TrySpendEnergyOrShowTired(int energyCost)
    {
        if (!HasEnoughEnergy(energyCost))
        {
            ShowTiredDialog();
            return false;
        }

        return SpendEnergy(energyCost);
    }

    /// <summary>
    /// Установить текущую энергию (для отладки или загрузки сохранения)
    /// </summary>
    /// <param name="energy">Новое значение энергии</param>
    public void SetEnergy(int energy)
    {
        currentEnergy = Mathf.Clamp(energy, 0, maxEnergy);
        Debug.Log($"[DayManager] Энергия установлена: {currentEnergy}/{maxEnergy}");
    }

    #endregion
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