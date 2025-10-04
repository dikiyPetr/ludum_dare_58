using UnityEngine;

public class CabinetSlot : MonoBehaviour
{
    private GameObject currentClueObject;
    private ClueData currentClue;

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

        currentClue = clueData;

        Debug.Log($"Улика '{clueData.id}' помещена в ячейку");
    }

    // Очистить ячейку
    public void ClearSlot()
    {
        if (currentClueObject != null)
        {
            Destroy(currentClueObject);
            currentClueObject = null;
            currentClue = null;
        }
    }
}