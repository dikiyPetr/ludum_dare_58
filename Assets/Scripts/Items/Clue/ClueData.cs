using System.Collections.Generic;
using UnityEngine;

// ScriptableObject для данных улики
[CreateAssetMenu(fileName = "NewClue", menuName = "Game/Clue")]
public class ClueData : ScriptableObject
{
    public string id;
    public string title;
    [TextArea(3, 10)]
    public string description;
    public GameObject cluePrefab; // Модель улики
    public List<string> referencesToClues = new List<string>(); // ID улик, на которые ссылается
    
    [Header("Display Settings")]
    public int cabinetSlotIndex;
}