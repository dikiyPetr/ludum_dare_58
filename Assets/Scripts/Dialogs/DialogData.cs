using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogs
{
    /// <summary>
    /// Основная структура диалога
    /// </summary>
    [Serializable]
    public class Dialog
    {
        public string id;
        public string startNodeId;
        public string speaker;
        public List<DialogNode> nodes = new List<DialogNode>();
    }

    /// <summary>
    /// Узел диалога с текстом и вариантами ответов
    /// </summary>
    [Serializable]
    public class DialogNode
    {
        public string id;
        public string text;
        public List<DialogOption> options = new List<DialogOption>();
        public List<NotebookEntry> notebookEntries = new List<NotebookEntry>();
        public List<DialogHighlight> highlights = new List<DialogHighlight>();
    }

    /// <summary>
    /// Вариант ответа игрока
    /// </summary>
    [Serializable]
    public class DialogOption
    {
        public string text;
        public string nextNodeId;
        public Condition condition;
    }

    /// <summary>
    /// Запись в блокнот/улика
    /// </summary>
    [Serializable]
    public class NotebookEntry
    {
        public string clueId;
        public string description;
    }

    /// <summary>
    /// Базовый класс для условий
    /// </summary>
    [Serializable]
    public abstract class Condition
    {
        public abstract bool Evaluate();
    }

    /// <summary>
    /// Условие наличия улики
    /// </summary>
    [Serializable]
    public class HasEvidence : Condition
    {
        public string id;

        public override bool Evaluate()
        {
            if (ClueManager.Instance != null)
            {
                return ClueManager.Instance.HasClue(id);
            }
            return false;
        }
    }

    /// <summary>
    /// Условие обнаруженной связи между уликами
    /// </summary>
    [Serializable]
    public class HasConnection : Condition
    {
        public string id;

        public override bool Evaluate()
        {
            if (ClueManager.Instance != null)
            {
                return ClueManager.Instance.IsConnectionDiscoveredById(id);
            }
            return false;
        }
    }

    /// <summary>
    /// Составное условие (AND/OR)
    /// </summary>
    [Serializable]
    public class MultiCondition : Condition
    {
        public enum LogicType
        {
            AND,
            OR
        }

        public LogicType logicType;
        public List<Condition> conditions = new List<Condition>();

        public override bool Evaluate()
        {
            if (conditions.Count == 0) return true;

            switch (logicType)
            {
                case LogicType.AND:
                    foreach (var condition in conditions)
                    {
                        if (!condition.Evaluate()) return false;
                    }
                    return true;

                case LogicType.OR:
                    foreach (var condition in conditions)
                    {
                        if (condition.Evaluate()) return true;
                    }
                    return false;

                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// Условие отрицания (NOT)
    /// </summary>
    [Serializable]
    public class NotCondition : Condition
    {
        public Condition condition;

        public override bool Evaluate()
        {
            if (condition == null) return true;
            return !condition.Evaluate();
        }
    }

    /// <summary>
    /// Подсветка части текста с тултипами
    /// </summary>
    [Serializable]
    public class DialogHighlight
    {
        public string word;
        public List<Tooltip> tooltips = new List<Tooltip>();
    }

    /// <summary>
    /// Тултип с условием показа
    /// </summary>
    [Serializable]
    public class Tooltip
    {
        public Condition condition;
        public string text;
    }

    /// <summary>
    /// Контейнер для всех диалогов
    /// </summary>
    [Serializable]
    public class DialogContainer
    {
        public List<Dialog> dialogs = new List<Dialog>();
    }
}
