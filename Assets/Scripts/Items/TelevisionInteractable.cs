using UnityEngine;

/// <summary>
/// Компонент для телевизора - запуск диалога с новостями
/// </summary>
public class TelevisionInteractable : MonoBehaviour, IOutlineInteractable
{
    [Header("Настройки")]
    [SerializeField] private string noNewsMessage = "Новостей пока нет";
    [SerializeField] private string hasNewsMessage = "Посмотреть новости";

    /// <summary>
    /// Показать информацию в overlay при наведении
    /// </summary>
    public void ShowOverlayInfo(OverlayInfoManager overlayInfo)
    {
        if (DayManager.Instance != null && DayManager.Instance.HasNewsForCurrentDay())
        {
            overlayInfo.ShowInfo(hasNewsMessage);
        }
        else
        {
            overlayInfo.ShowInfo(noNewsMessage);
        }
    }

    /// <summary>
    /// Обработать клик по объекту
    /// </summary>
    public bool OnClick()
    {
        // Клик обрабатывается только если есть новости для текущего дня
        if (DayManager.Instance == null)
        {
            Debug.LogWarning("[TelevisionInteractable] DayManager не найден!");
            return false;
        }

        if (!DayManager.Instance.HasNewsForCurrentDay())
        {
            Debug.Log("[TelevisionInteractable] Нет новостей для текущего дня");
            return false;
        }

        string newsDialogId = DayManager.Instance.GetNewsDialogIdForCurrentDay();

        if (string.IsNullOrEmpty(newsDialogId))
        {
            Debug.LogWarning("[TelevisionInteractable] ID диалога с новостями пуст!");
            return false;
        }

        // Запустить диалог с новостями
        if (Dialogs.DialogManager.Instance != null)
        {
            Debug.Log($"[TelevisionInteractable] Запуск новостей дня {DayManager.Instance.CurrentDay}: {newsDialogId}");
            Dialogs.DialogManager.Instance.StartDialog(newsDialogId);
            return true;
        }
        else
        {
            Debug.LogError("[TelevisionInteractable] DialogManager не найден!");
            return false;
        }
    }
}