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
    public System.Action<string> OnSuspectCaught;
    public System.Action<string> OnSuspectEliminated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeSuspects();
        }
        else
        {
            Destroy(gameObject);
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

    // Проверить всех подозреваемых на возможность открытия
    public void CheckAllSuspectsForUnlock()
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

    // Поймать подозреваемого
    public bool CatchSuspect(string suspectId)
    {
        if (!suspects.ContainsKey(suspectId))
        {
            Debug.LogWarning($"Подозреваемый с ID '{suspectId}' не найден!");
            return false;
        }

        SuspectState state = suspects[suspectId];

        // Если уже пойман
        if (state.isCaught)
        {
            return false;
        }

        // Ловим подозреваемого
        state.isCaught = true;
        Debug.Log($"<color=red>★ Подозреваемый '{state.data.suspectName}' пойман!</color>");

        // Вызываем событие
        OnSuspectCaught?.Invoke(suspectId);

        return true;
    }

    // Проверить, пойман ли подозреваемый
    public bool IsSuspectCaught(string suspectId)
    {
        if (suspects.ContainsKey(suspectId))
        {
            return suspects[suspectId].isCaught;
        }
        return false;
    }

    // Получить всех пойманных подозреваемых
    public List<SuspectState> GetCaughtSuspects()
    {
        List<SuspectState> caught = new List<SuspectState>();
        foreach (var suspect in suspects.Values)
        {
            if (suspect.isCaught)
            {
                caught.Add(suspect);
            }
        }
        return caught;
    }

    // Устранить подозреваемого
    public bool EliminateSuspect(string suspectId)
    {
        if (!suspects.ContainsKey(suspectId))
        {
            Debug.LogWarning($"Подозреваемый с ID '{suspectId}' не найден!");
            return false;
        }

        SuspectState state = suspects[suspectId];

        // Проверяем, что подозреваемый пойман
        if (!state.isCaught)
        {
            Debug.LogWarning($"Подозреваемый '{state.data.suspectName}' не пойман!");
            return false;
        }

        // Проверяем, что еще не устранён
        if (state.isEliminated)
        {
            Debug.LogWarning($"Подозреваемый '{state.data.suspectName}' уже устранён!");
            return false;
        }

        // Устраняем подозреваемого
        state.isEliminated = true;
        state.isCaught = false; // Больше не пойманный
        Debug.Log($"<color=red>★ Подозреваемый '{state.data.suspectName}' устранён!</color>");

        // Вызываем событие
        OnSuspectCaught.Invoke(suspectId);
        OnSuspectEliminated?.Invoke(suspectId);

        return true;
    }

    // Проверить, устранён ли подозреваемый
    public bool IsSuspectEliminated(string suspectId)
    {
        if (suspects.ContainsKey(suspectId))
        {
            return suspects[suspectId].isEliminated;
        }
        return false;
    }

    // Получить пойманного подозреваемого (только один может быть пойман)
    public SuspectState GetCaughtSuspect()
    {
        foreach (var suspect in suspects.Values)
        {
            if (suspect.isCaught)
            {
                return suspect;
            }
        }
        return null;
    }

    // Сбросить всех подозреваемых (для отладки)
    public void DebugResetAllSuspects()
    {
        foreach (var suspect in suspects.Values)
        {
            suspect.isRevealed = false;
            suspect.isCaught = false;
            suspect.isEliminated = false;
        }
        Debug.Log("[SuspectManager] Все подозреваемые сброшены!");
    }
}