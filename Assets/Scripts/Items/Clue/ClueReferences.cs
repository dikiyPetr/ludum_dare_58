using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClueReferences", menuName = "Game/Clue References")]
public class ClueReferences : ScriptableObject
{
    [Header("All Possible Connections")] [Tooltip("Все возможные связи между уликами")]
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

    public ClueConnection GetConnectionById(string connectionId)
    {
        foreach (var connection in allConnections)
        {
            if (connection.id == connectionId)
            {
                return connection;
            }
        }

        return null;
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