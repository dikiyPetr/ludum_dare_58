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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
                references = clueData.referencesToClues,
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
            OnClueCollected?.Invoke(clueId);
        }
        else
        {
            Debug.LogWarning($"Улика с ID '{clueId}' не найдена!");
        }
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

    // Получить связанные улики
    public List<string> GetRelatedClues(string clueId)
    {
        if (clues.ContainsKey(clueId))
        {
            return clues[clueId].references;
        }

        return new List<string>();
    }

    public void DebugClearAllClues()
    {
        foreach (var clue in clues.Values)
        {
            clue.hasClue = false;
        }

        Debug.Log("[ClueManager] Все улики сброшены!");
    }

    // События
    public System.Action<string> OnClueCollected;
}

// Класс состояния улики
[System.Serializable]
public class ClueState
{
    public string id;
    public bool hasClue;
    public string info;
    public List<string> references;
    public ClueData data; // Ссылка на ScriptableObject
}