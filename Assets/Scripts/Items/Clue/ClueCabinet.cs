using System.Collections.Generic;
using UnityEngine;

public class ClueCabinet : MonoBehaviour
{
    public static ClueCabinet Instance { get; private set; }

    [Header("Cabinet Slots")] [SerializeField]
    private List<CabinetSlot> slots = new List<CabinetSlot>();

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
        }
    }

    private void Start()
    {
        // Отобразить уже собранные улики
        RefreshAllClues();
    }

    private void OnDestroy()
    {
        if (ClueManager.Instance != null)
        {
            ClueManager.Instance.OnClueCollected -= OnClueCollected;
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

    // Очистить все ячейки
    public void ClearAllSlots()
    {
        foreach (CabinetSlot slot in slots)
        {
            slot.ClearSlot();
        }
    }
}