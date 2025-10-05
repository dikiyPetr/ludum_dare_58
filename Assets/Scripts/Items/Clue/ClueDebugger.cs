using System.Collections.Generic;
using UnityEngine;

public class ClueDebugger : MonoBehaviour
{
    [Header("Стартовые улики")] [Tooltip("Улики, которые будут выданы при старте игры")] [SerializeField]
    private List<ClueData> startingClues = new List<ClueData>();

    [Header("Настройки")] [SerializeField] private bool giveCluesOnStart = true;
    [SerializeField] private float delayBetweenClues = 0.1f; // Задержка между выдачей улик

    [Header("Горячие клавиши")] [SerializeField]
    private KeyCode giveAllCluesKey = KeyCode.F1;

    [SerializeField] private KeyCode clearAllCluesKey = KeyCode.F2;
    [SerializeField] private KeyCode printCluesKey = KeyCode.F3;
    [SerializeField] private KeyCode checkConnectionKey = KeyCode.F4;
    [SerializeField] private KeyCode catchFirstSuspectKey = KeyCode.F5;

    [Header("Быстрая выдача улик (для теста)")] [SerializeField]
    private List<QuickClue> quickClues = new List<QuickClue>();

    [Header("Автоматическое добавление связей")]
    [Tooltip("ClueReferencesData для автоматического добавления всех связей")]
    [SerializeField]
    private ClueReferencesData clueReferencesData;

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

        // F4 - проверить связь между тестовыми уликами
        if (Input.GetKeyDown(checkConnectionKey))
        {
            DiscoverAllConnectionsFromReferences();
        }

        // F5 - поймать первого подозреваемого
        if (Input.GetKeyDown(catchFirstSuspectKey))
        {
            CatchFirstSuspect();
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
            Debug.Log($"<color=white>• {clue.id}</color> - {clue.info}");
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
    
    // Попытка обнаружить связь между двумя уликами
    public void DiscoverConnection(string clueId1, string clueId2)
    {
        bool success = ClueManager.Instance.TryDiscoverConnection(clueId1, clueId2);

        if (success)
        {
            Debug.Log($"<color=green>[ClueDebugger]</color> ✓ Связь успешно обнаружена!");
        }
        else
        {
            Debug.Log($"<color=red>[ClueDebugger]</color> ✗ Связь не обнаружена (проверьте условия)");
        }
    }

    // Проверить, обнаружена ли связь
    public void CheckConnectionStatus(string clueId1, string clueId2)
    {
        bool discovered = ClueManager.Instance.IsConnectionDiscovered(clueId1, clueId2);
        string status = discovered ? "<color=green>✓ ОБНАРУЖЕНА</color>" : "<color=yellow>○ НЕ ОБНАРУЖЕНА</color>";
        Debug.Log($"<color=cyan>[ClueDebugger]</color> Связь '{clueId1}' <-> '{clueId2}': {status}");
    }

    // Проверить, обнаружена ли связь по ID
    public void CheckConnectionStatusById(string connectionId)
    {
        bool discovered = ClueManager.Instance.IsConnectionDiscoveredById(connectionId);
        string status = discovered ? "<color=green>✓ ОБНАРУЖЕНА</color>" : "<color=yellow>○ НЕ ОБНАРУЖЕНА</color>";
        Debug.Log($"<color=cyan>[ClueDebugger]</color> Связь с ID '{connectionId}': {status}");
    }

    // Обнаружить связь по ID
    public void DiscoverConnectionById(string connectionId)
    {
        bool success = ClueManager.Instance.TryDiscoverConnectionById(connectionId);

        if (success)
        {
            Debug.Log($"<color=green>[ClueDebugger]</color> ✓ Связь '{connectionId}' успешно обнаружена!");
        }
        else
        {
            Debug.Log($"<color=red>[ClueDebugger]</color> ✗ Связь '{connectionId}' не обнаружена (проверьте условия)");
        }
    }

    // Вывести все обнаруженные связи
    public void PrintDiscoveredConnections()
    {
        List<ClueConnection> connections = ClueManager.Instance.GetDiscoveredConnections();

        Debug.Log($"<color=cyan>[ClueDebugger]</color> === Обнаруженные связи ({connections.Count}) ===");

        if (connections.Count == 0)
        {
            Debug.Log($"<color=yellow>[ClueDebugger]</color> Нет обнаруженных связей");
            return;
        }

        foreach (var connection in connections)
        {
            bool isDiscovered = ClueManager.Instance.IsConnectionDiscovered(connection.clueId1, connection.clueId2);
            string marker = isDiscovered ? "✓" : "○";
            Debug.Log(
                $"<color=white>{marker} {connection.clueId1}</color> <-> <color=white>{connection.clueId2}</color>");
        }
    }

    // Добавить все связи из ClueReferencesData
    public void DiscoverAllConnectionsFromReferences()
    {
        if (clueReferencesData == null)
        {
            Debug.LogWarning($"<color=yellow>[ClueDebugger]</color> ClueReferencesData не назначен!");
            return;
        }

        if (clueReferencesData.allConnections == null || clueReferencesData.allConnections.Count == 0)
        {
            Debug.LogWarning($"<color=yellow>[ClueDebugger]</color> В ClueReferencesData нет связей!");
            return;
        }

        Debug.Log(
            $"<color=cyan>[ClueDebugger]</color> Добавление {clueReferencesData.allConnections.Count} связей из ClueReferencesData...");

        int successCount = 0;
        int failCount = 0;

        foreach (var connection in clueReferencesData.allConnections)
        {
            bool success = ClueManager.Instance.TryDiscoverConnection(connection.clueId1, connection.clueId2);
            if (success)
            {
                successCount++;
            }
            else
            {
                failCount++;
            }
        }

        Debug.Log($"<color=green>[ClueDebugger]</color> Связи добавлены: {successCount} успешно, {failCount} неудачно");
    }

    // Поймать первого подозреваемого
    public void CatchFirstSuspect()
    {
        if (SuspectManager.Instance == null)
        {
            Debug.LogWarning($"<color=yellow>[ClueDebugger]</color> SuspectManager не найден!");
            return;
        }

        var revealedSuspects = SuspectManager.Instance.GetRevealedSuspects();

        if (revealedSuspects == null || revealedSuspects.Count == 0)
        {
            Debug.LogWarning($"<color=yellow>[ClueDebugger]</color> Нет открытых подозреваемых для поимки!");
            return;
        }

        SuspectState firstSuspect = revealedSuspects[0];
        bool success = SuspectManager.Instance.CatchSuspect(firstSuspect.id);

        if (success)
        {
            Debug.Log($"<color=green>[ClueDebugger]</color> ✓ Подозреваемый '{firstSuspect.data.suspectName}' (ID: {firstSuspect.id}) пойман!");
        }
        else
        {
            Debug.Log($"<color=yellow>[ClueDebugger]</color> Подозреваемый '{firstSuspect.data.suspectName}' уже был пойман");
        }
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