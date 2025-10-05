using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs
{
    /// <summary>
    /// UI компонент для отображения стопки тултипов
    /// </summary>
    public class TooltipUI : MonoBehaviour
    {
        [Header("UI Контейнеры")]
        [SerializeField] private RectTransform contentContainer;
        [SerializeField] private VerticalLayoutGroup layoutGroup;
        [SerializeField] private CanvasGroup canvasGroup;
        
        [Header("Настройки отображения")]
        [SerializeField] private float padding = 10f;
        [SerializeField] private float spacing = 5f;
        [SerializeField] private float minWidth = 150f;
        [SerializeField] private float maxWidth = 400f;
        [SerializeField] private int fontSize = 14;
        [SerializeField] private Color textColor = Color.white;
        
        private RectTransform rectTransform;
        private List<TMP_Text> textElements = new List<TMP_Text>();

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
            
            SetupLayout();
            Hide();
        }

        /// <summary>
        /// Настройка layout если не задано
        /// </summary>
        private void SetupLayout()
        {
            if (contentContainer == null)
            {
                contentContainer = rectTransform;
            }
            
            if (layoutGroup == null && contentContainer != null)
            {
                layoutGroup = contentContainer.GetComponent<VerticalLayoutGroup>();
                if (layoutGroup == null)
                {
                    layoutGroup = contentContainer.gameObject.AddComponent<VerticalLayoutGroup>();
                }
            }
            
            if (layoutGroup != null)
            {
                layoutGroup.padding = new RectOffset((int)padding, (int)padding, (int)padding, (int)padding);
                layoutGroup.spacing = spacing;
                layoutGroup.childAlignment = TextAnchor.UpperLeft;
                layoutGroup.childControlWidth = true;
                layoutGroup.childControlHeight = true;
                layoutGroup.childForceExpandWidth = false;
                layoutGroup.childForceExpandHeight = false;
            }
            
            // ContentSizeFitter для автоматического размера
            var fitter = contentContainer.GetComponent<ContentSizeFitter>();
            if (fitter == null)
            {
                fitter = contentContainer.gameObject.AddComponent<ContentSizeFitter>();
            }
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        /// <summary>
        /// Показать тултипы с массивом текстов
        /// </summary>
        public void Show(List<string> texts)
        {
            ClearTexts();
            
            if (texts == null || texts.Count == 0)
            {
                Hide();
                return;
            }

            foreach (var text in texts)
            {
                CreateTextElement(text);
            }
            
            gameObject.SetActive(true);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
            
            // Форсируем обновление layout
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentContainer);
            
            // Применяем ограничения ширины
            ApplyWidthConstraints();
        }

        /// <summary>
        /// Создать текстовый элемент
        /// </summary>
        private void CreateTextElement(string text)
        {
            var textObj = new GameObject("TooltipText", typeof(RectTransform));
            textObj.transform.SetParent(contentContainer, false);
            
            var textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = fontSize;
            textComponent.color = textColor;
            textComponent.alignment = TextAlignmentOptions.TopLeft;
            textComponent.textWrappingMode = TextWrappingModes.Normal;
            
            // LayoutElement для контроля размера
            var layoutElement = textObj.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = maxWidth - padding * 2;
            layoutElement.flexibleWidth = 0;
            
            textElements.Add(textComponent);
        }

        /// <summary>
        /// Применить ограничения ширины
        /// </summary>
        private void ApplyWidthConstraints()
        {
            if (rectTransform == null) return;
            
            float currentWidth = rectTransform.sizeDelta.x;
            float clampedWidth = Mathf.Clamp(currentWidth, minWidth, maxWidth);
            
            if (!Mathf.Approximately(currentWidth, clampedWidth))
            {
                rectTransform.sizeDelta = new Vector2(clampedWidth, rectTransform.sizeDelta.y);
            }
        }

        /// <summary>
        /// Очистить все текстовые элементы
        /// </summary>
        private void ClearTexts()
        {
            foreach (var textElement in textElements)
            {
                if (textElement != null)
                {
                    Destroy(textElement.gameObject);
                }
            }
            textElements.Clear();
        }

        /// <summary>
        /// Скрыть тултип
        /// </summary>
        public void Hide()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Установить якорную позицию
        /// </summary>
        public void SetAnchoredPosition(Vector2 position)
        {
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = position;
            }
        }
        
        /// <summary>
        /// Получить текущий размер
        /// </summary>
        public Vector2 GetSize()
        {
            return rectTransform != null ? rectTransform.sizeDelta : Vector2.zero;
        }
    }
}

