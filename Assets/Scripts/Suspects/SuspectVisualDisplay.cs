using UnityEngine;

// Компонент для отображения визуальных элементов подозреваемого
public class SuspectVisualDisplay : MonoBehaviour, IOutlineInteractable
{
    [Header("Suspect Configuration")] [SerializeField]
    private SuspectData suspectState;

    [Header("Visual Components")] [SerializeField]
    private GameObject[] objectsToToggle; // Объекты для показа/скрытия

    [SerializeField]
    private GameObject[] objectsToToggleWhenEliminated; // Объекты для показа, когда подозреваемый убит
 // Объекты для скрытия, когда подозреваемый убит

    [Header("Display Settings")] [SerializeField]
    private bool hideWhenLocked = true; // Скрывать компоненты, когда подозреваемый закрыт

    private bool isRevealed = false;
    private bool isEliminated = false;

    private void Start()
    {
        // Подписываемся на события SuspectManager
        if (SuspectManager.Instance != null)
        {
            SuspectManager.Instance.OnSuspectRevealed += OnSuspectRevealed;
            SuspectManager.Instance.OnSuspectEliminated += OnSuspectEliminated;

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
            SuspectManager.Instance.OnSuspectEliminated -= OnSuspectEliminated;
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

    // Обработка устранения подозреваемого
    private void OnSuspectEliminated(string eliminatedSuspectId)
    {
        if (suspectState != null && eliminatedSuspectId == suspectState.id)
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
        isEliminated = SuspectManager.Instance.IsSuspectEliminated(suspectState.id);
        bool shouldShow = isRevealed || !hideWhenLocked;

        // Переключение GameObjects
        foreach (var obj in objectsToToggle)
        {
            if (obj != null)
                obj.SetActive(shouldShow);
        }

        // Показать объекты при устранении
        if (isEliminated)
        {
            foreach (var obj in objectsToToggleWhenEliminated)
            {
                if (obj != null)
                    obj.SetActive(true);
            }

        }
        else
        {
            // Если не устранён, скрываем объекты для устранённых
            foreach (var obj in objectsToToggleWhenEliminated)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
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

    public void ShowOverlayInfo(OverlayInfoManager overlayInfo)
    {
        if (suspectState != null)
        {
            overlayInfo.ShowSuspectOverlay(suspectState);
        }
    }

    public bool OnClick()
    {
        if (suspectState != null && !string.IsNullOrEmpty(suspectState.mapDialogNodeId))
        {
            if (Dialogs.DialogManager.Instance != null && !Dialogs.DialogManager.Instance.IsInDialog)
            {
                Dialogs.DialogManager.Instance.StartDialog(suspectState.mapDialogNodeId);
                return true;
            }
        }
        return false;
    }
}