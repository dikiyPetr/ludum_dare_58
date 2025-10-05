using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dialogs
{
    /// <summary>
    /// Менеджер системы подсветки и тултипов
    /// </summary>
    public class HighlightManager : MonoBehaviour
    {
        public static HighlightManager Instance { get; private set; }

        // События
        public static event Action<DialogNode> OnHighlightsUpdated;
        public static event Action<string, List<Tooltip>> OnTooltipRequested;
        public static event Action OnTooltipsHidden;

        // Текущие данные
        private DialogNode currentNode;
        private Dictionary<string, List<Tooltip>> currentTooltips = new Dictionary<string, List<Tooltip>>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            // Подписка на события диалогов
            DialogManager.OnNodePlayed += OnNodePlayed;
        }

        private void OnDisable()
        {
            // Отписка от событий
            DialogManager.OnNodePlayed -= OnNodePlayed;
        }

        /// <summary>
        /// Обработка смены узла диалога
        /// </summary>
        private void OnNodePlayed(DialogNode node)
        {
            currentNode = node;
            UpdateHighlights();
        }

        /// <summary>
        /// Обновить подсветки для текущего узла
        /// </summary>
        private void UpdateHighlights()
        {
            currentTooltips.Clear();

            if (currentNode?.highlights == null) return;

            foreach (var highlight in currentNode.highlights)
            {
                if (string.IsNullOrEmpty(highlight.word)) continue;

                // Фильтруем тултипы по условиям
                var availableTooltips = new List<Tooltip>();
                foreach (var tooltip in highlight.tooltips)
                {
                    if (ConditionEvaluator.Evaluate(tooltip.condition))
                    {
                        availableTooltips.Add(tooltip);
                    }
                }

                if (availableTooltips.Count > 0)
                {
                    currentTooltips[highlight.word] = availableTooltips;
                }
            }

            OnHighlightsUpdated?.Invoke(currentNode);
        }

        /// <summary>
        /// Запросить тултипы для слова
        /// </summary>
        public void RequestTooltips(string word)
        {
            if (string.IsNullOrEmpty(word)) return;

            if (currentTooltips.TryGetValue(word, out var tooltips))
            {
                OnTooltipRequested?.Invoke(word, tooltips);
            }
            else
            {
                Debug.LogWarning($"Тултипы для слова '{word}' не найдены");
            }
        }

        /// <summary>
        /// Скрыть все тултипы
        /// </summary>
        public void HideTooltips()
        {
            OnTooltipsHidden?.Invoke();
        }

        /// <summary>
        /// Получить все текущие тултипы
        /// </summary>
        public Dictionary<string, List<Tooltip>> GetCurrentTooltips()
        {
            return new Dictionary<string, List<Tooltip>>(currentTooltips);
        }

        /// <summary>
        /// Получить тултипы для конкретного слова
        /// </summary>
        public List<Tooltip> GetTooltipsForWord(string word)
        {
            if (currentTooltips.TryGetValue(word, out var tooltips))
            {
                return new List<Tooltip>(tooltips);
            }
            return new List<Tooltip>();
        }

        /// <summary>
        /// Проверить, есть ли подсветки для слова
        /// </summary>
        public bool HasHighlight(string word)
        {
            return currentTooltips.ContainsKey(word);
        }

        /// <summary>
        /// Получить все подсвеченные слова
        /// </summary>
        public List<string> GetHighlightedWords()
        {
            return new List<string>(currentTooltips.Keys);
        }

        /// <summary>
        /// Получить количество доступных тултипов для слова
        /// </summary>
        public int GetTooltipCount(string word)
        {
            if (currentTooltips.TryGetValue(word, out var tooltips))
            {
                return tooltips.Count;
            }
            return 0;
        }

        /// <summary>
        /// Обновить подсветки принудительно
        /// </summary>
        public void RefreshHighlights()
        {
            UpdateHighlights();
        }
    }
}
