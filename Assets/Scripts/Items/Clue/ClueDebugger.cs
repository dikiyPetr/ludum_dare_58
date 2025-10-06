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

    private void Start()
    {
        if (giveCluesOnStart && startingClues.Count > 0)
        {
            StartCoroutine(GiveStartingClues());
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
}

