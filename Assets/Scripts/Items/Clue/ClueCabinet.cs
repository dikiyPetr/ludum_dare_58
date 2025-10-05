using System.Collections.Generic;
using UnityEngine;

public class ClueCabinet : MonoBehaviour
{
    public static ClueCabinet Instance { get; private set; }

    [Header("Cabinet Slots")] [SerializeField]
    private List<CabinetSlot> slots = new List<CabinetSlot>();

    [Header("Connection Visualization")]
    [SerializeField] private ClueConnectionRenderer connectionRenderer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Подписка на события системы улик
        if (ClueManager.Instance != null)
        {
            ClueManager.Instance.OnClueCollected += OnClueCollected;
            ClueManager.Instance.OnConnectionDiscovered += OnConnectionDiscovered;
        }
    }

    private void Start()
    {
        // Отобразить уже собранные улики
        RefreshAllClues();

        // Отобразить уже обнаруженные связи
        RefreshAllConnections();
    }

    private void OnDestroy()
    {
        if (ClueManager.Instance != null)
        {
            ClueManager.Instance.OnClueCollected -= OnClueCollected;
            ClueManager.Instance.OnConnectionDiscovered -= OnConnectionDiscovered;
        }
    }

    // Обработка сбора новой улики
    private void OnClueCollected(string clueId)
    {
        ClueState clueState = ClueManager.Instance.GetClueState(clueId);
        if (clueState?.data != null)
        {
            DisplayClue(clueState.data);
        }
    }

    // Обработка обнаружения новой связи (через события, для обратной совместимости)
    private void OnConnectionDiscovered(string clueId1, string clueId2)
    {
        DrawConnection(clueId1, clueId2);
    }

    // Нарисовать связь между двумя уликами (вызывается напрямую из ClueManager)
    public void DrawConnection(string clueId1, string clueId2)
    {
        if (connectionRenderer == null)
        {
            Debug.LogWarning("ClueConnectionRenderer не назначен!");
            return;
        }

        // Найти ячейки для обеих улик
        CabinetSlot slot1 = FindSlotByClueId(clueId1);
        CabinetSlot slot2 = FindSlotByClueId(clueId2);

        if (slot1 != null && slot2 != null)
        {
            // Создать ключ для связи
            string connectionKey = GetConnectionKey(clueId1, clueId2);

            // Получить позиции ячеек
            Vector3 pos1 = slot1.transform.position;
            Vector3 pos2 = slot2.transform.position;

            // Отрисовать линию
            connectionRenderer.CreateConnectionLine(connectionKey, pos1, pos2);
        }
    }

    // Найти ячейку по ID улики
    private CabinetSlot FindSlotByClueId(string clueId)
    {
        foreach (var slot in slots)
        {
            ClueData clueData = slot.GetClue();
            if (clueData != null && clueData.id == clueId)
            {
                return slot;
            }
        }
        return null;
    }

    // Получить ключ связи (такой же как в ClueManager)
    private string GetConnectionKey(string id1, string id2)
    {
        if (string.Compare(id1, id2) < 0)
            return $"{id1}_{id2}";
        return $"{id2}_{id1}";
    }

    // Отобразить улику в шкафу
    public void DisplayClue(ClueData clueData)
    {
        if (clueData == null) return;

        CabinetSlot targetSlot = GetSlotByIndex(clueData.cabinetSlotIndex);

        if (targetSlot != null)
        {
            targetSlot.PlaceClue(clueData);
        }
        else
        {
            Debug.LogWarning($"Ячейка с индексом {clueData.cabinetSlotIndex} не найдена!");
        }
    }

    // Получить ячейку по индексу
    public CabinetSlot GetSlotByIndex(int index)
    {
        return slots[index];
    }

    // Обновить отображение всех улик
    public void RefreshAllClues()
    {
        List<ClueState> collectedClues = ClueManager.Instance.GetCollectedClues();

        foreach (ClueState clueState in collectedClues)
        {
            if (clueState.data != null)
            {
                DisplayClue(clueState.data);
            }
        }
    }

    // Обновить отображение всех связей
    public void RefreshAllConnections()
    {
        if (connectionRenderer == null || ClueManager.Instance == null) return;

        // Получить все обнаруженные связи
        List<ClueConnection> discoveredConnections = ClueManager.Instance.GetDiscoveredConnections();

        foreach (var connection in discoveredConnections)
        {
            // Найти ячейки для обеих улик
            CabinetSlot slot1 = FindSlotByClueId(connection.clueId1);
            CabinetSlot slot2 = FindSlotByClueId(connection.clueId2);

            if (slot1 != null && slot2 != null)
            {
                string connectionKey = GetConnectionKey(connection.clueId1, connection.clueId2);

                // Отрисовать линию, если её ещё нет
                if (!connectionRenderer.HasLine(connectionKey))
                {
                    Vector3 pos1 = slot1.transform.position;
                    Vector3 pos2 = slot2.transform.position;
                    connectionRenderer.CreateConnectionLine(connectionKey, pos1, pos2);
                }
            }
        }
    }

    // Очистить все ячейки
    public void ClearAllSlots()
    {
        foreach (CabinetSlot slot in slots)
        {
            slot.ClearSlot();
        }

        // Очистить все линии связей
        if (connectionRenderer != null)
        {
            connectionRenderer.ClearAllLines();
        }
    }
}