using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dialogs
{
    /// <summary>
    /// Компонент для подсветки слов в TextMeshPro и обработки наведения/кликов
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class TextHighlighter : MonoBehaviour, IPointerMoveHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Настройки подсветки")]
        [SerializeField] private Color highlightColor = new Color(1f, 0.8f, 0.2f, 1f);
        
        private TMP_Text textComponent;
        private Dictionary<string, List<int>> wordToLinkIndices = new Dictionary<string, List<int>>();
        private string currentHoveredWord = null;
        private int currentLinkIndex = -1;

        private void Awake()
        {
            textComponent = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            // Подписываемся на обновления подсветок
            HighlightManager.OnHighlightsUpdated += OnHighlightsUpdated;
            DialogManager.OnNodePlayed += OnNodePlayed;
        }

        private void OnDisable()
        {
            HighlightManager.OnHighlightsUpdated -= OnHighlightsUpdated;
            DialogManager.OnNodePlayed -= OnNodePlayed;
        }

        /// <summary>
        /// Обработка обновления подсветок
        /// </summary>
        private void OnHighlightsUpdated(DialogNode node)
        {
            // Применяем подсветки с небольшой задержкой, чтобы DialogUIController успел установить текст
            StartCoroutine(ApplyHighlightsDelayed());
        }

        /// <summary>
        /// Обработка смены узла диалога
        /// </summary>
        private void OnNodePlayed(DialogNode node)
        {
            ApplyHighlights();
        }
        
        private System.Collections.IEnumerator ApplyHighlightsDelayed()
        {
            yield return null; // Ждем один кадр
            ApplyHighlights();
        }

        /// <summary>
        /// Применить подсветки к тексту
        /// </summary>
        private void ApplyHighlights()
        {
            if (textComponent == null || HighlightManager.Instance == null) return;

            wordToLinkIndices.Clear();
            
            var highlightedWords = HighlightManager.Instance.GetHighlightedWords();
            if (highlightedWords.Count == 0)
            {
                // Если нет подсветок, показать текст как есть
                if (DialogManager.Instance != null && DialogManager.Instance.CurrentNode != null)
                {
                    textComponent.text = DialogManager.Instance.CurrentNode.text;
                }
                return;
            }

            // Получить оригинальный текст
            string originalText = DialogManager.Instance?.CurrentNode?.text ?? textComponent.text;
            
            // Применить rich text теги для подсветки
            string highlightedText = originalText;
            int linkIndex = 0;

            foreach (var word in highlightedWords)
            {
                if (string.IsNullOrEmpty(word)) continue;

                // Найти все вхождения слова (case-insensitive)
                int startIndex = 0;
                while ((startIndex = highlightedText.IndexOf(word, startIndex, System.StringComparison.OrdinalIgnoreCase)) != -1)
                {
                    // Проверить, что это целое слово, а не часть другого
                    bool isWholeWord = true;
                    if (startIndex > 0 && char.IsLetterOrDigit(highlightedText[startIndex - 1]))
                    {
                        isWholeWord = false;
                    }
                    if (startIndex + word.Length < highlightedText.Length && 
                        char.IsLetterOrDigit(highlightedText[startIndex + word.Length]))
                    {
                        isWholeWord = false;
                    }

                    if (isWholeWord)
                    {
                        // Обернуть слово в теги подсветки и ссылки
                        string before = highlightedText.Substring(0, startIndex);
                        string wordText = highlightedText.Substring(startIndex, word.Length);
                        string after = highlightedText.Substring(startIndex + word.Length);

                        string colorHex = ColorUtility.ToHtmlStringRGBA(highlightColor);
                        string wrappedWord = $"<link=\"{linkIndex}\"><color=#{colorHex}><u>{wordText}</u></color></link>";
                        highlightedText = before + wrappedWord + after;

                        // Сохранить связь слова с индексом ссылки
                        if (!wordToLinkIndices.ContainsKey(word))
                        {
                            wordToLinkIndices[word] = new List<int>();
                        }
                        wordToLinkIndices[word].Add(linkIndex);

                        linkIndex++;
                        startIndex += wrappedWord.Length;
                    }
                    else
                    {
                        startIndex += word.Length;
                    }
                }
            }

            textComponent.text = highlightedText;
        }

        /// <summary>
        /// Обработка наведения мыши
        /// </summary>
        public void OnPointerMove(PointerEventData eventData)
        {
            if (textComponent == null) return;

            // Определить индекс символа под курсором
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, eventData.position, null);

            if (linkIndex != -1)
            {
                // Получить слово по индексу ссылки
                string word = GetWordByLinkIndex(linkIndex);
                
                if (word != null && word != currentHoveredWord)
                {
                    currentHoveredWord = word;
                    currentLinkIndex = linkIndex;
                    
                    // Запросить показ тултипов
                    if (HighlightManager.Instance != null)
                    {
                        HighlightManager.Instance.RequestTooltips(word);
                    }
                }
            }
            else if (currentHoveredWord != null)
            {
                // Курсор ушел со слова
                currentHoveredWord = null;
                currentLinkIndex = -1;
                
                if (HighlightManager.Instance != null)
                {
                    HighlightManager.Instance.HideTooltips();
                }
            }
        }

        /// <summary>
        /// Обработка выхода курсора за пределы текста
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (currentHoveredWord != null)
            {
                currentHoveredWord = null;
                currentLinkIndex = -1;
                
                if (HighlightManager.Instance != null)
                {
                    HighlightManager.Instance.HideTooltips();
                }
            }
        }

        /// <summary>
        /// Обработка клика по тексту
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (textComponent == null) return;

            int linkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, eventData.position, null);

            if (linkIndex != -1)
            {
                string word = GetWordByLinkIndex(linkIndex);
                
                if (word != null)
                {
                    Debug.Log($"Клик по подсвеченному слову: {word}");
                    // Здесь можно добавить дополнительную логику при клике
                }
            }
        }

        /// <summary>
        /// Получить слово по индексу ссылки
        /// </summary>
        private string GetWordByLinkIndex(int linkIndex)
        {
            foreach (var kvp in wordToLinkIndices)
            {
                if (kvp.Value.Contains(linkIndex))
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// Принудительное обновление подсветок
        /// </summary>
        public void RefreshHighlights()
        {
            ApplyHighlights();
        }
    }
}

