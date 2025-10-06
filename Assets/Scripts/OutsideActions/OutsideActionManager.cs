using UnityEngine;

/// <summary>
/// Менеджер действий снаружи дома
/// </summary>
public class OutsideActionManager : MonoBehaviour, IOutlineInteractable
{
    public static OutsideActionManager Instance { get; private set; }

    // ID катсцены для воспроизведения при выходе из дома
    private string pendingCutsceneId = null;

    // Событие, вызываемое при выходе из дома
    public System.Action<string> OnExitHouse;

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
    /// Сохранить ID катсцены для воспроизведения при выходе из дома
    /// </summary>
    public void SetPendingCutscene(string cutsceneId)
    {
        pendingCutsceneId = cutsceneId;
        Debug.Log($"[OutsideActionManager] Установлена катсцена для выхода: {cutsceneId}");
    }

    /// <summary>
    /// Получить ID сохранённой катсцены
    /// </summary>
    public string GetPendingCutscene()
    {
        return pendingCutsceneId;
    }

    /// <summary>
    /// Проверить, есть ли сохранённая катсцена
    /// </summary>
    public bool HasPendingCutscene()
    {
        return !string.IsNullOrEmpty(pendingCutsceneId);
    }

    /// <summary>
    /// Очистить сохранённую катсцену
    /// </summary>
    public void ClearPendingCutscene()
    {
        Debug.Log($"[OutsideActionManager] Очищена катсцена: {pendingCutsceneId}");
        pendingCutsceneId = null;
    }

    /// <summary>
    /// Вызвать событие выхода из дома (запускает сохранённую катсцену, если есть)
    /// </summary>
    public void ExitHouse()
    {
        if (HasPendingCutscene())
        {
            if (!DayManager.Instance.SpendEnergy(1))
            {
                return;
            }

            string cutsceneToPlay = pendingCutsceneId;
            ClearPendingCutscene();
            Debug.Log($"[OutsideActionManager] Выход из дома, запуск катсцены: {cutsceneToPlay}");

            // Вызываем событие
            OnExitHouse?.Invoke(cutsceneToPlay);

            // Запускаем катсцену
            if (Cutscenes.CutsceneManager.Instance != null)
            {
                Cutscenes.CutsceneManager.Instance.StartCutscene(cutsceneToPlay);
            }
            else
            {
                Debug.LogError("[OutsideActionManager] CutsceneManager не найден!");
            }
        }
        else
        {
            Debug.LogWarning("[OutsideActionManager] Нет сохранённой катсцены для выхода из дома!");
        }
    }

    #region IOutlineInteractable Implementation

    /// <summary>
    /// Показать информацию в overlay при наведении
    /// </summary>
    public void ShowOverlayInfo(OverlayInfoManager overlayInfo)
    {
        if (HasPendingCutscene())
        {
            overlayInfo.ShowInfo("Пора выйти");
        }
        else
        {
            overlayInfo.ShowInfo("Сначала нужно выбрать, что делать дальше");
        }
    }

    /// <summary>
    /// Обработать клик по объекту
    /// </summary>
    public bool OnClick()
    {
        // Клик обрабатывается только если есть pending cutscene
        if (HasPendingCutscene())
        {
            ExitHouse();
            return true;
        }

        return false;
    }

    #endregion
}