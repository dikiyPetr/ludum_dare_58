using System.Collections.Generic;
using UnityEngine;

// Компонент для отрисовки красных линий между связанными уликами
public class ClueConnectionRenderer : MonoBehaviour
{
    [Header("Line Settings")]
    [SerializeField] private Material lineMaterial;
    [SerializeField] private Color connectionColor = Color.red;
    [SerializeField] private float lineWidth = 0.02f;

    [Header("3D Offset Settings")]
    [Tooltip("Смещение линии от центра объекта (для видимости в 3D)")]
    [SerializeField] private Vector3 positionOffset = new Vector3(0, 0.1f, 0);

    private Dictionary<string, LineRenderer> activeLines = new Dictionary<string, LineRenderer>();

    // Создать линию между двумя точками
    public void CreateConnectionLine(string connectionKey, Vector3 startPos, Vector3 endPos)
    {
        // Если линия уже существует, удалить её
        if (activeLines.ContainsKey(connectionKey))
        {
            RemoveConnectionLine(connectionKey);
        }

        // Создать новый GameObject для LineRenderer
        GameObject lineObj = new GameObject($"Connection_{connectionKey}");
        lineObj.transform.SetParent(transform);

        LineRenderer line = lineObj.AddComponent<LineRenderer>();

        // Настроить LineRenderer
        line.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        line.startColor = connectionColor;
        line.endColor = connectionColor;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = 2;
        line.useWorldSpace = true;

        // Установить позиции с учётом offset
        line.SetPosition(0, startPos + positionOffset);
        line.SetPosition(1, endPos + positionOffset);

        // Сохранить ссылку
        activeLines[connectionKey] = line;

        Debug.Log($"Создана линия связи: {connectionKey}");
    }

    // Удалить линию связи
    public void RemoveConnectionLine(string connectionKey)
    {
        if (activeLines.ContainsKey(connectionKey))
        {
            Destroy(activeLines[connectionKey].gameObject);
            activeLines.Remove(connectionKey);
            Debug.Log($"Удалена линия связи: {connectionKey}");
        }
    }

    // Обновить позицию линии
    public void UpdateConnectionLine(string connectionKey, Vector3 startPos, Vector3 endPos)
    {
        if (activeLines.ContainsKey(connectionKey))
        {
            LineRenderer line = activeLines[connectionKey];
            line.SetPosition(0, startPos + positionOffset);
            line.SetPosition(1, endPos + positionOffset);
        }
    }

    // Очистить все линии
    public void ClearAllLines()
    {
        foreach (var line in activeLines.Values)
        {
            if (line != null)
            {
                Destroy(line.gameObject);
            }
        }
        activeLines.Clear();
    }

    // Проверить, существует ли линия
    public bool HasLine(string connectionKey)
    {
        return activeLines.ContainsKey(connectionKey);
    }
}