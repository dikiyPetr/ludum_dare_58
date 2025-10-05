using UnityEngine;

// Компонент для отображения визуальных элементов подозреваемого
public class SuspectVisualDisplay : MonoBehaviour
{
    [Header("Suspect Configuration")] [SerializeField]
    private SuspectData suspectState;

    [Header("Visual Components")] [SerializeField]
    private GameObject[] objectsToToggle; // Объекты для показа/скрытия

    [Header("Display Settings")] [SerializeField]
    private bool hideWhenLocked = true; // Скрывать компоненты, когда подозреваемый закрыт

    private bool isRevealed = false;

    private void Start()
    {
        // Подписываемся на события SuspectManager
        if (SuspectManager.Instance != null)
        {
            SuspectManager.Instance.OnSuspectRevealed += OnSuspectRevealed;

            // Проверяем текущее состояние подозреваемого
            UpdateVisualState();
        }
        else
        {
            Debug.LogWarning($"[SuspectVisualDisplay] SuspectManager не найден для подозреваемого");
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от событий
        if (SuspectManager.Instance != null)
        {
            SuspectManager.Instance.OnSuspectRevealed -= OnSuspectRevealed;
        }
    }

    // Обработка открытия подозреваемого
    private void OnSuspectRevealed(string revealedSuspectId)
    {
        if (suspectState != null && revealedSuspectId == suspectState.id)
        {
            UpdateVisualState();
        }
    }

    // Обновить визуальное состояние всех компонентов
    private void UpdateVisualState()
    {
        if (suspectState == null || SuspectManager.Instance == null)
            return;

        isRevealed = SuspectManager.Instance.IsSuspectRevealed(suspectState.id);
        bool shouldShow = isRevealed || !hideWhenLocked;

        // Переключение GameObjects
        foreach (var obj in objectsToToggle)
        {
            if (obj != null)
                obj.SetActive(shouldShow);
        }
    }

    // Принудительное обновление состояния (для вызова извне)
    public void RefreshDisplay()
    {
        UpdateVisualState();
    }

    // Получить данные подозреваемого
    public SuspectData GetSuspectData()
    {
        return suspectState;
    }

    // Проверить, открыт ли подозреваемый
    public bool IsRevealed()
    {
        return isRevealed;
    }
}