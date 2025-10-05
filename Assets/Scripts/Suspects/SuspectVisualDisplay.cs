using UnityEngine;

// Компонент для отображения визуальных элементов подозреваемого
public class SuspectVisualDisplay : MonoBehaviour
{
    [Header("Suspect Configuration")]
    [SerializeField] private string suspectId; // ID подозреваемого, с которым связан этот визуальный компонент

    [Header("Visual Components")]
    [SerializeField] private GameObject[] objectsToToggle; // Объекты для показа/скрытия

    [Header("Display Settings")]
    [SerializeField] private bool hideWhenLocked = true; // Скрывать компоненты, когда подозреваемый закрыт

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
            Debug.LogWarning($"[SuspectVisualDisplay] SuspectManager не найден для подозреваемого '{suspectId}'");
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
        if (revealedSuspectId == suspectId)
        {
            UpdateVisualState();
        }
    }

    // Обновить визуальное состояние всех компонентов
    private void UpdateVisualState()
    {
        if (SuspectManager.Instance == null)
            return;

        isRevealed = SuspectManager.Instance.IsSuspectRevealed(suspectId);
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

    // Получить ID подозреваемого
    public string GetSuspectId()
    {
        return suspectId;
    }

    // Проверить, открыт ли подозреваемый
    public bool IsRevealed()
    {
        return isRevealed;
    }
}