using System;
using System.Collections.Generic;
using UnityEngine;

// ScriptableObject для данных улики
[CreateAssetMenu(fileName = "NewClue", menuName = "Game/Clue")]
public class ClueData : ScriptableObject
{
    public string id;
    public string title;
    [TextArea(3, 10)] public string description;
    public GameObject cluePrefab; // Модель улики

    [Header("Display Settings")] public int cabinetSlotIndex;
}

[System.Serializable]
public class ClueConnection
{
    public string clueId1;
    public string clueId2;

    public ClueConnection(string id1, string id2)
    {
        clueId1 = id1;
        clueId2 = id2;
    }

    public bool ContainsClue(string clueId)
    {
        return clueId1 == clueId || clueId2 == clueId;
    }

    public string GetOtherClue(string clueId)
    {
        if (clueId1 == clueId) return clueId2;
        if (clueId2 == clueId) return clueId1;
        return null;
    }
}

[CreateAssetMenu(fileName = "ClueReferences", menuName = "Game/Clue References")]
public class ClueReferencesData : ScriptableObject
{
    [Header("All Possible Connections")]
    [Tooltip("Все возможные связи между уликами")]
    public List<ClueConnection> allConnections = new List<ClueConnection>();

    public bool HasConnection(string clueId1, string clueId2)
    {
        foreach (var connection in allConnections)
        {
            if ((connection.clueId1 == clueId1 && connection.clueId2 == clueId2) ||
                (connection.clueId1 == clueId2 && connection.clueId2 == clueId1))
            {
                return true;
            }
        }
        return false;
    }

    public List<string> GetConnectedClues(string clueId)
    {
        List<string> connected = new List<string>();
        foreach (var connection in allConnections)
        {
            if (connection.ContainsClue(clueId))
            {
                string other = connection.GetOtherClue(clueId);
                if (other != null)
                {
                    connected.Add(other);
                }
            }
        }
        return connected;
    }
}