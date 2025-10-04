using System.Collections.Generic;
using UnityEngine;

public class ClueDebugger : MonoBehaviour
{
    [Header("Стартовые улики")]
    [Tooltip("Улики, которые будут выданы при старте игры")]
    [SerializeField] private List<ClueData> startingClues = new List<ClueData>();
    
    [Header("Настройки")]
    [SerializeField] private bool giveCluesOnStart = true;
    [SerializeField] private float delayBetweenClues = 0.1f; // Задержка между выдачей улик
    
    [Header("Горячие клавиши")]
    [SerializeField] private KeyCode giveAllCluesKey = KeyCode.F1;
    [SerializeField] private KeyCode clearAllCluesKey = KeyCode.F2;
    [SerializeField] private KeyCode printCluesKey = KeyCode.F3;

    [Header("Быстрая выдача улик (для теста)")]
    [SerializeField] private List<QuickClue> quickClues = new List<QuickClue>();

    private void Start()
    {
        if (giveCluesOnStart && startingClues.Count > 0)
        {
            StartCoroutine(GiveStartingClues());
        }
    }

    private void Update()
    {
        // F1 - выдать все стартовые улики
        if (Input.GetKeyDown(giveAllCluesKey))
        {
            GiveAllStartingClues();
        }

        // F2 - очистить все улики (для тестов)
        if (Input.GetKeyDown(clearAllCluesKey))
        {
            ClearAllClues();
        }

        // F3 - вывести список собранных улик
        if (Input.GetKeyDown(printCluesKey))
        {
            PrintCollectedClues();
        }

        // Быстрая выдача улик по кнопкам
        foreach (var quickClue in quickClues)
        {
            if (Input.GetKeyDown(quickClue.key))
            {
                GiveClue(quickClue.clue);
            }
        }
    }

    // Выдать стартовые улики с задержкой
    private System.Collections.IEnumerator GiveStartingClues()
    {
        Debug.Log($"<color=cyan>[ClueDebugger]</color> Выдача {startingClues.Count} стартовых улик...");
        
        foreach (ClueData clue in startingClues)
        {
            if (clue != null)
            {
                ClueManager.Instance.AddClue(clue.id);
                yield return new WaitForSeconds(delayBetweenClues);
            }
        }
        
        Debug.Log($"<color=green>[ClueDebugger]</color> Стартовые улики выданы!");
    }

    // Выдать все стартовые улики мгновенно
    public void GiveAllStartingClues()
    {
        Debug.Log($"<color=cyan>[ClueDebugger]</color> Выдача всех стартовых улик...");
        
        foreach (ClueData clue in startingClues)
        {
            if (clue != null)
            {
                ClueManager.Instance.AddClue(clue.id);
            }
        }
        
        Debug.Log($"<color=green>[ClueDebugger]</color> Выдано {startingClues.Count} улик!");
    }

    // Выдать конкретную улику
    public void GiveClue(ClueData clue)
    {
        if (clue != null)
        {
            ClueManager.Instance.AddClue(clue.id);
            Debug.Log($"<color=green>[ClueDebugger]</color> Выдана улика: {clue.title} (ID: {clue.id})");
        }
    }

    // Выдать улику по ID
    public void GiveClueById(string clueId)
    {
        ClueManager.Instance.AddClue(clueId);
        Debug.Log($"<color=green>[ClueDebugger]</color> Выдана улика с ID: {clueId}");
    }

    // Очистить все улики (для тестов - придется переделать ClueManager)
    public void ClearAllClues()
    {
        Debug.Log($"<color=yellow>[ClueDebugger]</color> Очистка всех улик и обновление шкафа...");
        
        // Очистить шкаф
        if (ClueCabinet.Instance != null)
        {
            ClueCabinet.Instance.ClearAllSlots();
        }
        
        // Для полной очистки нужно добавить метод в ClueManager
        Debug.Log($"<color=yellow>[ClueDebugger]</color> Шкаф очищен. Для полного сброса перезагрузите сцену.");
    }

    // Вывести список собранных улик
    public void PrintCollectedClues()
    {
        List<ClueState> collected = ClueManager.Instance.GetCollectedClues();
        
        Debug.Log($"<color=cyan>[ClueDebugger]</color> === Собранные улики ({collected.Count}) ===");
        
        foreach (ClueState clue in collected)
        {
            string references = clue.references.Count > 0 
                ? $" → [{string.Join(", ", clue.references)}]" 
                : "";
            
            Debug.Log($"<color=white>• {clue.id}</color> - {clue.info}{references}");
        }
    }

    // Добавить улику в список стартовых через код
    public void AddToStartingClues(ClueData clue)
    {
        if (!startingClues.Contains(clue))
        {
            startingClues.Add(clue);
            Debug.Log($"<color=cyan>[ClueDebugger]</color> Улика '{clue.id}' добавлена в стартовые");
        }
    }

    // Проверить наличие улики
    public void CheckClue(string clueId)
    {
        bool hasClue = ClueManager.Instance.HasClue(clueId);
        string status = hasClue ? "<color=green>✓ ЕСТЬ</color>" : "<color=red>✗ НЕТ</color>";
        Debug.Log($"<color=cyan>[ClueDebugger]</color> Улика '{clueId}': {status}");
    }
}

// Класс для быстрой выдачи улик по кнопкам
[System.Serializable]
public class QuickClue
{
    public KeyCode key;
    public ClueData clue;
    public string description; // Описание для удобства в инспекторе
}
