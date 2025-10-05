using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs
{
    /// <summary>
    /// Позиция тултипа относительно курсора
    /// </summary>
    public enum TooltipPosition
    {
        Below,        // Ниже курсора
        Above,        // Выше курсора
        Right,        // Справа от курсора
        Left,         // Слева от курсора
        Custom        // Пользовательское смещение
    }

    /// <summary>
    /// Менеджер системы тултипов
    /// Управляет отображением тултипов для подсвеченных слов в диалогах
    /// </summary>
    public class TooltipManager : MonoBehaviour
    {
        public static TooltipManager Instance { get; private set; }

        [Header("Настройки")]
        [SerializeField] private TooltipUI tooltipUI;
        
        [Header("Позиционирование")]
        [SerializeField] private TooltipPosition tooltipPosition = TooltipPosition.Below;
        [SerializeField] private float distanceFromCursor = 10f;
        [SerializeField] private Vector2 customOffset = new Vector2(10f, 10f);
        
        private Canvas tooltipCanvas;
        private RectTransform tooltipContainer;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Автоконфигурация
            tooltipCanvas = GetComponentInParent<Canvas>();
            tooltipContainer = GetComponent<RectTransform>();

            // Создать tooltip UI если не задан
            if (tooltipUI == null)
            {
                tooltipUI = CreateDefaultTooltipUI();
            }
        }

        private void OnEnable()
        {
            // Подписаться на события подсветок
            HighlightManager.OnTooltipRequested += ShowTooltips;
            HighlightManager.OnTooltipsHidden += HideAllTooltips;
        }

        private void OnDisable()
        {
            HighlightManager.OnTooltipRequested -= ShowTooltips;
            HighlightManager.OnTooltipsHidden -= HideAllTooltips;
        }

        /// <summary>
        /// Показать тултипы для слова
        /// </summary>
        private void ShowTooltips(string word, List<Tooltip> tooltips)
        {
            if (tooltips == null || tooltips.Count == 0)
            {
                HideAllTooltips();
                return;
            }

            // Собираем все тексты в один список
            List<string> texts = new List<string>();
            foreach (var tooltip in tooltips)
            {
                texts.Add(tooltip.text);
            }
            
            // Показываем все тултипы в одном UI
            tooltipUI.Show(texts);

            // Получаем размер тултипа после отображения
            Vector2 tooltipSize = tooltipUI.GetSize();
            
            // Позиционирование относительно курсора
            Vector2 mousePosition = Input.mousePosition;
            Vector2 offset = CalculateOffset(tooltipSize);
            Vector2 screenPosition = mousePosition + offset;
            
            // Конвертация screen space в canvas space
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                tooltipContainer, 
                screenPosition, 
                tooltipCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : tooltipCanvas.worldCamera,
                out Vector2 localPoint
            );
            
            tooltipUI.SetAnchoredPosition(localPoint);
        }

        /// <summary>
        /// Вычислить смещение тултипа относительно курсора
        /// Учитывает pivot (0, 1) - левый верхний угол
        /// </summary>
        private Vector2 CalculateOffset(Vector2 tooltipSize)
        {
            switch (tooltipPosition)
            {
                case TooltipPosition.Below:
                    // Ниже курсора: левый верхний угол чуть ниже курсора
                    return new Vector2(0f, -distanceFromCursor);
                
                case TooltipPosition.Above:
                    // Выше курсора: нижний край выше курсора
                    return new Vector2(0f, distanceFromCursor + tooltipSize.y);
                
                case TooltipPosition.Right:
                    // Справа от курсора: левый край справа от курсора, верх на уровне курсора
                    return new Vector2(distanceFromCursor, 0f);
                
                case TooltipPosition.Left:
                    // Слева от курсора: правый край слева от курсора, верх на уровне курсора
                    return new Vector2(-distanceFromCursor - tooltipSize.x, 0f);
                
                case TooltipPosition.Custom:
                    return customOffset;
                
                default:
                    return new Vector2(0f, -distanceFromCursor);
            }
        }

        /// <summary>
        /// Скрыть все тултипы
        /// </summary>
        public void HideAllTooltips()
        {
            if (tooltipUI != null)
            {
                tooltipUI.Hide();
            }
        }

        /// <summary>
        /// Создать стандартный tooltip UI
        /// </summary>
        private TooltipUI CreateDefaultTooltipUI()
        {
            // Создать объект тултипа
            GameObject tooltipObj = new GameObject("TooltipUI", typeof(RectTransform), typeof(CanvasGroup));
            tooltipObj.transform.SetParent(tooltipContainer, false);
            
            var rectTransform = tooltipObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0f, 1f);

            // Фон
            var bgImage = tooltipObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);

            // TooltipUI компонент
            var ui = tooltipObj.AddComponent<TooltipUI>();
            tooltipObj.SetActive(false);

            return ui;
        }

        /// <summary>
        /// Показать тултипы вручную (для тестирования)
        /// </summary>
        public void ShowTooltipsManual(string word)
        {
            if (HighlightManager.Instance != null)
            {
                HighlightManager.Instance.RequestTooltips(word);
            }
        }
    }
}

