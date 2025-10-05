using System.Collections.Generic;
using UnityEngine;

// Менеджер системы подозреваемых
public class SuspectManager : MonoBehaviour
{
    public static SuspectManager Instance { get; private set; }

    [Header("Suspects")]
    [SerializeField] private List<SuspectData> allSuspects = new List<SuspectData>();

    // Словарь: ID подозреваемого -> состояние
    private Dictionary<string, SuspectState> suspects = new Dictionary<string, SuspectState>();

    // События
    public System.Action<string> OnSuspectRevealed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSuspects();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Подписываемся на события ClueManager
        if (ClueManager.Instance != null)
        {
            ClueManager.Instance.OnClueCollected += OnClueCollected;
            ClueManager.Instance.OnConnectionDiscovered += OnConnectionDiscovered;
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от событий
        if (ClueManager.Instance != null)
        {
            ClueManager.Instance.OnClueCollected -= OnClueCollected;
            ClueManager.Instance.OnConnectionDiscovered -= OnConnectionDiscovered;
        }
    }

    // Инициализация всех подозреваемых
    private void InitializeSuspects()
    {
        foreach (var suspectData in allSuspects)
        {
            suspects[suspectData.id] = new SuspectState(suspectData.id, suspectData);
        }
    }

    // Проверить условия открытия подозреваемого
    public bool CheckUnlockConditions(string suspectId)
    {
        if (!suspects.ContainsKey(suspectId))
        {
            Debug.LogWarning($"Подозреваемый с ID '{suspectId}' не найден!");
            return false;
        }

        SuspectState state = suspects[suspectId];
        SuspectData data = state.data;

        // Проверяем, что все необходимые улики собраны
        foreach (string clueId in data.requiredClueIds)
        {
            if (!ClueManager.Instance.HasClue(clueId))
            {
                return false;
            }
        }

        // Проверяем, что все необходимые связи обнаружены
        foreach (string connectionId in data.requiredConnectionIds)
        {
            if (!ClueManager.Instance.IsConnectionDiscoveredById(connectionId))
            {
                return false;
            }
        }

        return true;
    }

    // Попытаться открыть подозреваемого
    public bool TryRevealSuspect(string suspectId)
    {
        if (!suspects.ContainsKey(suspectId))
        {
            Debug.LogWarning($"Подозреваемый с ID '{suspectId}' не найден!");
            return false;
        }

        SuspectState state = suspects[suspectId];

        // Если уже открыт
        if (state.isRevealed)
        {
            return false;
        }

        // Проверяем условия
        if (!CheckUnlockConditions(suspectId))
        {
            return false;
        }

        // Открываем подозреваемого
        state.isRevealed = true;
        Debug.Log($"<color=yellow>★ Подозреваемый '{state.data.suspectName}' открыт!</color>");

        // Вызываем событие
        OnSuspectRevealed?.Invoke(suspectId);

        return true;
    }

    // Проверить, открыт ли подозреваемый
    public bool IsSuspectRevealed(string suspectId)
    {
        if (suspects.ContainsKey(suspectId))
        {
            return suspects[suspectId].isRevealed;
        }
        return false;
    }

    // Получить состояние подозреваемого
    public SuspectState GetSuspectState(string suspectId)
    {
        if (suspects.ContainsKey(suspectId))
        {
            return suspects[suspectId];
        }
        return null;
    }

    // Получить всех открытых подозреваемых
    public List<SuspectState> GetRevealedSuspects()
    {
        List<SuspectState> revealed = new List<SuspectState>();
        foreach (var suspect in suspects.Values)
        {
            if (suspect.isRevealed)
            {
                revealed.Add(suspect);
            }
        }
        return revealed;
    }

    // Обработка сбора улики
    private void OnClueCollected(string clueId)
    {
        CheckAllSuspectsForUnlock();
    }

    // Обработка обнаружения связи
    private void OnConnectionDiscovered(string clueId1, string clueId2)
    {
        CheckAllSuspectsForUnlock();
    }

    // Проверить всех подозреваемых на возможность открытия
    private void CheckAllSuspectsForUnlock()
    {
        foreach (var suspect in suspects.Values)
        {
            if (!suspect.isRevealed)
            {
                TryRevealSuspect(suspect.id);
            }
        }
    }

    // Принудительно открыть подозреваемого (для отладки или сюжетных событий)
    public void ForceRevealSuspect(string suspectId)
    {
        if (!suspects.ContainsKey(suspectId))
        {
            Debug.LogWarning($"Подозреваемый с ID '{suspectId}' не найден!");
            return;
        }

        SuspectState state = suspects[suspectId];
        if (state.isRevealed)
        {
            return;
        }

        state.isRevealed = true;
        Debug.Log($"<color=yellow>★ Подозреваемый '{state.data.suspectName}' принудительно открыт!</color>");

        OnSuspectRevealed?.Invoke(suspectId);
    }

    // Сбросить всех подозреваемых (для отладки)
    public void DebugResetAllSuspects()
    {
        foreach (var suspect in suspects.Values)
        {
            suspect.isRevealed = false;
        }
        Debug.Log("[SuspectManager] Все подозреваемые сброшены!");
    }
}