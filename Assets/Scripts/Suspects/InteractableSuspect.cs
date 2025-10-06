using UnityEngine;

// Интерактивный компонент подозреваемого
public class InteractableSuspect : MonoBehaviour, IOutlineInteractable
{
    private SuspectData suspectData;

    // Установить данные подозреваемого
    public void SetSuspectData(SuspectData data)
    {
        suspectData = data;
        OnSuspectDataSet();
    }

    // Получить данные подозреваемого
    public SuspectData GetSuspectData()
    {
        return suspectData;
    }

    // Вызывается когда данные подозреваемого установлены
    protected virtual void OnSuspectDataSet()
    {
        // Переопределяется в наследниках для реакции на установку данных
    }

    public void ShowOverlayInfo(OverlayInfoManager overlayInfo)
    {
        if (suspectData != null)
        {
            overlayInfo.ShowSuspectOverlay(suspectData);
        }
    }

    public bool OnClick()
    {
        if (suspectData != null && !string.IsNullOrEmpty(suspectData.suspectDialogNodeId))
        {
            if (Dialogs.DialogManager.Instance != null && !Dialogs.DialogManager.Instance.IsInDialog)
            {
                Dialogs.DialogManager.Instance.StartDialog(suspectData.suspectDialogNodeId);
                return true;
            }
        }
        return false;
    }

    public void OnHoverEnter() { }
    public void OnHoverExit() { }
    public void OnPressEnter() { }
    public void OnReleaseEnter() { }
}