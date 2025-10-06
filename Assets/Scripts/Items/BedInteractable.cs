using Dialogs;
using UnityEngine;

/// <summary>
/// Компонент для кровати - пропуск дня
/// </summary>
public class BedInteractable : MonoBehaviour, IOutlineInteractable
{
    [Header("Настройки")]
    [SerializeField] private string message = "Go to sleep";

    /// <summary>
    /// Показать информацию в overlay при наведении
    /// </summary>
    public void ShowOverlayInfo(OverlayInfoManager overlayInfo)
    {
        overlayInfo.ShowInfo(message);
    }

    /// <summary>
    /// Обработать клик по объекту
    /// </summary>
    public bool OnClick()
    {
        if (DayManager.Instance == null)
        {
            Debug.LogWarning("[BedInteractable] DayManager не найден!");
            return false;
        }

        // Пропустить день
        Debug.Log($"[BedInteractable] Пропуск дня {DayManager.Instance.CurrentDay}");
        DialogManager.Instance.StartDialog("skip_day");

        return true;
    }
}