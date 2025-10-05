using UnityEngine;

// Интерактивный компонент подозреваемого
public class InteractableSuspect : MonoBehaviour
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
}