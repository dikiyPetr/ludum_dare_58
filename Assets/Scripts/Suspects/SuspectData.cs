using System.Collections.Generic;
using UnityEngine;

// ScriptableObject для данных подозреваемого
[CreateAssetMenu(fileName = "NewSuspect", menuName = "Game/Suspect")]
public class SuspectData : ScriptableObject
{
    [Header("Basic Info")]
    public string id;
    public string suspectName;
    [TextArea(3, 10)] public string description;

    [Header("Unlock Conditions")]
    [Tooltip("Улики, необходимые для открытия подозреваемого")]
    public List<string> requiredClueIds = new List<string>();

    [Tooltip("Связи между уликами, необходимые для открытия")]
    public List<string> requiredConnectionIds = new List<string>();
}

// Класс состояния подозреваемого
[System.Serializable]
public class SuspectState
{
    public string id;
    public bool isRevealed; // Открыт ли подозреваемый
    public SuspectData data; // Ссылка на ScriptableObject

    public SuspectState(string id, SuspectData data)
    {
        this.id = id;
        this.isRevealed = false;
        this.data = data;
    }
}