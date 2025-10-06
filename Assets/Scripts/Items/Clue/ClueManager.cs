using System.Collections.Generic;
using UnityEngine;

// Менеджер системы улик
public class ClueManager : MonoBehaviour
{
    public static ClueManager Instance { get; private set; }

    // Словарь: ID улики -> состояние (имеется/не имеется)
    private Dictionary<string, ClueState> clues = new Dictionary<string, ClueState>();

    // Все доступные улики в игре
    [SerializeField] private List<ClueData> allClues = new List<ClueData>();

    // Ссылка на данные связей между уликами
    [SerializeField] private ClueReferencesData clueReferences;
    
    // Ссылка на ClueCabinet для прямой передачи улик и связей
    [SerializeField] private ClueCabinet clueCabinet;

    // Ссылка на SuspectManager для обновления подозреваемых
    [SerializeField] private SuspectManager suspectManager;

    // Обнаруженные игроком связи
    private HashSet<string> discoveredConnections = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeClues();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    

    // Инициализация всех улик
    private void InitializeClues()
    {
        foreach (var clueData in allClues)
        {
            clues[clueData.id] = new ClueState
            {
                id = clueData.id,
                hasClue = false,
                info = clueData.description,
                data = clueData
            };
        }
    }

    // Добавить улику в инвентарь
    public void AddClue(string clueId)
    {
        if (clues.ContainsKey(clueId))
        {
            clues[clueId].hasClue = true;
            Debug.Log($"Улика '{clueId}' добавлена!");

            // Передать улику напрямую в ClueCabinet
            if (clueCabinet != null && clues[clueId].data != null)
            {
                clueCabinet.DisplayClue(clues[clueId].data);
            }

            // Обновить SuspectManager
            if (suspectManager != null)
            {
                suspectManager.CheckAllSuspectsForUnlock();
            }

            OnClueCollected?.Invoke(clueId);
        }
        else
        {
            Debug.LogWarning($"Улика с ID '{clueId}' не найдена!");
        }
    }

    // Проверить связь между двумя уликами (вызывается игроком)
    public bool TryDiscoverConnection(string clueId1, string clueId2)
    {
        if (clueReferences == null)
        {
            Debug.LogWarning("ClueReferencesData не назначен!");
            return false;
        }

        // Проверить, что обе улики собраны
        if (!HasClue(clueId1) || !HasClue(clueId2))
        {
            Debug.Log($"Обе улики должны быть собраны для проверки связи!");
            return false;
        }

        // Проверить, есть ли связь в данных
        if (!clueReferences.HasConnection(clueId1, clueId2))
        {
            Debug.Log($"Связи между '{clueId1}' и '{clueId2}' не существует.");
            return false;
        }

        // Проверить, не была ли связь уже обнаружена
        string connectionKey = GetConnectionKey(clueId1, clueId2);
        if (discoveredConnections.Contains(connectionKey))
        {
            Debug.Log($"Связь между '{clueId1}' и '{clueId2}' уже обнаружена!");
            return false;
        }

        // Обнаружить связь
        discoveredConnections.Add(connectionKey);
        Debug.Log($"<color=green>✓ Обнаружена связь между '{clueId1}' и '{clueId2}'!</color>");

        // Передать связь напрямую в ClueCabinet
        if (clueCabinet != null)
        {
            clueCabinet.DrawConnection(clueId1, clueId2);
        }

        // Обновить SuspectManager
        if (suspectManager != null)
        {
            suspectManager.CheckAllSuspectsForUnlock();
        }

        // Показать диалог с описанием связи
        ShowConnectionDialog(clueId1, clueId2);

        OnConnectionDiscovered?.Invoke(clueId1, clueId2);
        return true;
    }

    // Получить уникальный ключ для связи (сортированный)
    private string GetConnectionKey(string id1, string id2)
    {
        if (string.Compare(id1, id2) < 0)
            return $"{id1}_{id2}";
        return $"{id2}_{id1}";
    }

    // Получить все обнаруженные связи
    public List<ClueConnection> GetDiscoveredConnections()
    {
        if (clueReferences == null) return new List<ClueConnection>();

        List<ClueConnection> discovered = new List<ClueConnection>();

        foreach (var connection in clueReferences.allConnections)
        {
            // Проверить, что обе улики собраны
            if (HasClue(connection.clueId1) && HasClue(connection.clueId2))
            {
                discovered.Add(connection);
            }
        }

        return discovered;
    }

    // Проверить, открыта ли связь между уликами
    public bool IsConnectionDiscovered(string clueId1, string clueId2)
    {
        string key = GetConnectionKey(clueId1, clueId2);
        return discoveredConnections.Contains(key);
    }

    // Проверить, открыта ли связь по ID
    public bool IsConnectionDiscoveredById(string connectionId)
    {
        if (clueReferences == null) return false;

        var connection = clueReferences.GetConnectionById(connectionId);
        if (connection == null) return false;

        return IsConnectionDiscovered(connection.clueId1, connection.clueId2);
    }

    // Обнаружить связь по ID
    public bool TryDiscoverConnectionById(string connectionId)
    {
        if (clueReferences == null) return false;

        var connection = clueReferences.GetConnectionById(connectionId);
        if (connection == null)
        {
            Debug.LogWarning($"Связь с ID '{connectionId}' не найдена!");
            return false;
        }

        return TryDiscoverConnection(connection.clueId1, connection.clueId2);
    }

    // Проверить наличие улики
    public bool HasClue(string clueId)
    {
        return clues.ContainsKey(clueId) && clues[clueId].hasClue;
    }

    // Получить информацию об улике
    public ClueState GetClueState(string clueId)
    {
        if (clues.ContainsKey(clueId))
        {
            return clues[clueId];
        }

        return null;
    }

    // Получить все собранные улики
    public List<ClueState> GetCollectedClues()
    {
        List<ClueState> collected = new List<ClueState>();
        foreach (var clue in clues.Values)
        {
            if (clue.hasClue)
            {
                collected.Add(clue);
            }
        }

        return collected;
    }

    // Проверить наличие нескольких улик (для диалогов)
    public bool HasAllClues(params string[] clueIds)
    {
        foreach (string id in clueIds)
        {
            if (!HasClue(id))
                return false;
        }

        return true;
    }

    // Проверить наличие хотя бы одной улики из списка
    public bool HasAnyClue(params string[] clueIds)
    {
        foreach (string id in clueIds)
        {
            if (HasClue(id))
                return true;
        }

        return false;
    }
    
    public void DebugClearAllClues()
    {
        foreach (var clue in clues.Values)
        {
            clue.hasClue = false;
        }

        Debug.Log("[ClueManager] Все улики сброшены!");
    }

    // Показать диалог с описанием связи между уликами
    private void ShowConnectionDialog(string clueId1, string clueId2)
    {
        if (Dialogs.DialogManager.Instance == null)
        {
            Debug.LogWarning("DialogManager не найден, невозможно показать диалог о связи");
            return;
        }

        // Получить данные связи
        ClueConnection connection = null;
        foreach (var conn in clueReferences.allConnections)
        {
            if ((conn.clueId1 == clueId1 && conn.clueId2 == clueId2) ||
                (conn.clueId1 == clueId2 && conn.clueId2 == clueId1))
            {
                connection = conn;
                break;
            }
        }

        if (connection == null || string.IsNullOrEmpty(connection.description))
        {
            Debug.LogWarning($"Описание связи между '{clueId1}' и '{clueId2}' не найдено");
            return;
        }

        // Создать кастомный диалог
        var customDialog = new Dialogs.Dialog
        {
            id = $"connection_{clueId1}_{clueId2}",
            startNodeId = "start",
            speaker = "Детектив",
            nodes = new List<Dialogs.DialogNode>
            {
                new Dialogs.DialogNode
                {
                    id = "start",
                    text = connection.description,
                    options = new List<Dialogs.DialogOption>
                    {
                        new Dialogs.DialogOption
                        {
                            text = "ОК",
                            nextNodeId = null // Завершить диалог
                        }
                    }
                }
            }
        };

        // Показать диалог
        Dialogs.DialogManager.Instance.ShowSimpleDialog(customDialog);
    }

    // События
    public System.Action<string> OnClueCollected;
    public System.Action<string, string> OnConnectionDiscovered;
}

// Класс состояния улики
[System.Serializable]
public class ClueState
{
    public string id;
    public bool hasClue;
    public string info;
    public ClueData data; // Ссылка на ScriptableObject
}