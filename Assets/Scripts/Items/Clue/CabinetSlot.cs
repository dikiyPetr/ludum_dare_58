using UnityEngine;

public class CabinetSlot : MonoBehaviour
{
    private GameObject currentClueObject;
    private ClueData currentClue;
    public ClueData GetClue() => currentClue;

    // Поместить улику в ячейку - инстанцирует префаб
    public void PlaceClue(ClueData clueData)
    {
        // Удалить предыдущую улику, если есть
        ClearSlot();

        if (clueData == null || clueData.cluePrefab == null)
        {
            Debug.LogWarning($"ClueData или prefab отсутствует для ячейки");
            return;
        }

        // Инстанцировать префаб улики прямо в эту ячейку
        currentClueObject = Instantiate(clueData.cluePrefab, transform);

        // Добавить компонент CabinetSlotInteractable в заспавненный префаб
        CabinetSlotInteractable slotInteractable = currentClueObject.AddComponent<CabinetSlotInteractable>();
        slotInteractable.Initialize(clueData);

        currentClue = clueData;

        Debug.Log($"Улика '{clueData.id}' помещена в ячейку");
    }

    // Очистить ячейку
    public void ClearSlot()
    {
        if (currentClueObject != null)
        {
            currentClue = null;
            Destroy(currentClueObject);
            currentClueObject = null;
        }
    }
}

public class CabinetSlotInteractable : MonoBehaviour, IOutlineInteractable
{
    private ClueData clueData;

    public void Initialize(ClueData data)
    {
        clueData = data;
    }

    public ClueData GetClue() => clueData;

    public void ShowOverlayInfo(OverlayInfoManager overlayInfo)
    {
        overlayInfo.ShowClueOverlay(clueData);
    }

    public bool OnClick()
    {
        // CabinetSlotInteractable не обрабатывает клики напрямую
        return false;
    }
}