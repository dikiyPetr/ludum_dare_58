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
    public string id; // Уникальный идентификатор связи для проверки в диалогах
    [TextArea(2, 5)] public string description; // Описание связи
    public string clueId1;
    public string clueId2;

    public ClueConnection()
    {
    }

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
    