using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs
{
    /// <summary>
    /// UI контроллер панели диалогов: отображает текст узла и варианты ответов
    /// </summary>
    public class DialogUIController : MonoBehaviour
    {
        [Header("UI-ссылки")]
        [SerializeField] private TMP_Text speakerText;
        [SerializeField] private TMP_Text dialogText;
        [SerializeField] private RectTransform optionsContainer;
        [SerializeField] private Button optionButtonPrefab;

        private readonly List<Button> spawnedButtons = new List<Button>();

        void Awake()
        {
            // Автоконфигурация, если ссылки не заданы в инспекторе
            if (dialogText == null)
            {
                dialogText = GetComponentInChildren<TMP_Text>(true);
            }

            if (false && optionsContainer == null)
            {
                var go = new GameObject("Options", typeof(RectTransform));
                var rt = go.GetComponent<RectTransform>();
                rt.SetParent(transform, false);
                rt.anchorMin = new Vector2(0f, 0f);
                rt.anchorMax = new Vector2(1f, 0f);
                rt.pivot = new Vector2(0.5f, 0f);
                rt.sizeDelta = new Vector2(0f, 160f);
                rt.anchoredPosition = new Vector2(0f, 10f);

                // Вертикальный список (необязательно, но удобно)
                go.AddComponent<VerticalLayoutGroup>();
                var fitter = go.AddComponent<ContentSizeFitter>();
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                optionsContainer = rt;
            }

            if (optionButtonPrefab == null)
            {
                optionButtonPrefab = CreateDefaultButtonTemplate();
                optionButtonPrefab.gameObject.SetActive(false);
            }
        }

        void OnEnable()
        {
            DialogManager.OnNodePlayed += HandleNodePlayed;
            DialogManager.OnDialogEnded += HandleDialogEnded;

            // Если диалог уже активен при включении панели — отрисовать текущий узел
            if (DialogManager.Instance != null && DialogManager.Instance.IsInDialog && DialogManager.Instance.CurrentNode != null)
            {
                RenderNode(DialogManager.Instance.CurrentDialog, DialogManager.Instance.CurrentNode);
            }
        }

        void OnDisable()
        {
            DialogManager.OnNodePlayed -= HandleNodePlayed;
            DialogManager.OnDialogEnded -= HandleDialogEnded;
        }

        private void HandleNodePlayed(DialogNode node)
        {
            if (DialogManager.Instance == null) return;
            RenderNode(DialogManager.Instance.CurrentDialog, node);
        }

        private void HandleDialogEnded(Dialog dialog)
        {
            ClearOptions();
            if (speakerText != null) speakerText.text = string.Empty;
            if (dialogText != null) dialogText.text = string.Empty;
        }

        private void RenderNode(Dialog dialog, DialogNode node)
        {
            if (node == null) return;

            if (speakerText != null)
            {
                speakerText.text = dialog != null ? dialog.speaker : string.Empty;
            }

            // Текст будет установлен через TextHighlighter (если он есть)
            // Иначе устанавливаем напрямую
            var highlighter = dialogText != null ? dialogText.GetComponent<TextHighlighter>() : null;
            if (highlighter == null && dialogText != null)
            {
                // Нет подсветки - просто устанавливаем текст
                dialogText.text = node.text ?? string.Empty;
            }
            // Если есть TextHighlighter, он сам установит текст с подсветками
            
            // Обновить подсветки для тултипов
            if (HighlightManager.Instance != null)
            {
                HighlightManager.Instance.RefreshHighlights();
            }

            // Кнопки вариантов
            ClearOptions();
            var optionsAll = node.options ?? new List<DialogOption>();
            var optionsAvailable = DialogManager.Instance != null
                ? DialogManager.Instance.GetAvailableOptions()
                : new List<DialogOption>();

            // Если есть доступные по условиям — показываем только их
            if (optionsAvailable.Count > 0)
            {
                for (int i = 0; i < optionsAvailable.Count; i++)
                {
                    int originalIndex = node.options.IndexOf(optionsAvailable[i]);
                    if (originalIndex < 0) originalIndex = i;
                    CreateOptionButton(originalIndex, optionsAvailable[i].text);
                }
                return;
            }

            // Иначе отображаем все варианты, недоступные делаем неактивными визуально
            for (int i = 0; i < optionsAll.Count; i++)
            {
                var btn = CreateOptionButton(i, optionsAll[i].text);
                if (btn != null)
                {
                    bool canSelect = ConditionEvaluator.Evaluate(optionsAll[i].condition);
                    btn.interactable = canSelect;
                }
            }
        }

        private Button CreateOptionButton(int index, string text)
        {
            // if (optionButtonPrefab == null || optionsContainer == null) return;

            var button = Instantiate(optionButtonPrefab, optionsContainer);
            button.gameObject.name = $"Option_{index}";

            var label = button.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
            {
                label.text = text ?? string.Empty;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SelectOption(index));

            button.gameObject.SetActive(true);
            spawnedButtons.Add(button);
            return button;
        }

        private void SelectOption(int index)
        {
            if (DialogManager.Instance == null) return;
            DialogManager.Instance.SelectOption(index);
        }

        private void ClearOptions()
        {
            foreach (var btn in spawnedButtons)
            {
                if (btn != null)
                {
                    Destroy(btn.gameObject);
                }
            }
            spawnedButtons.Clear();

            // Также удалим все оставшиеся дочерние элементы, если они есть
            if (optionsContainer != null)
            {
                for (int i = optionsContainer.childCount - 1; i >= 0; i--)
                {
                    Destroy(optionsContainer.GetChild(i).gameObject);
                }
            }
        }

        private Button CreateDefaultButtonTemplate()
        {
            var go = new GameObject("OptionButtonTemplate", typeof(RectTransform), typeof(Image), typeof(Button));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(transform, false);
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(1f, 0f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(0f, 40f);

            var image = go.GetComponent<Image>();
            image.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);

            var textGo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            var textRt = textGo.GetComponent<RectTransform>();
            textRt.SetParent(rt, false);
            textRt.anchorMin = new Vector2(0f, 0f);
            textRt.anchorMax = new Vector2(1f, 1f);
            textRt.offsetMin = new Vector2(10f, 5f);
            textRt.offsetMax = new Vector2(-10f, -5f);

            var tmp = textGo.GetComponent<TextMeshProUGUI>();
            tmp.text = "Option";
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 12f;
            tmp.fontSizeMax = 24f;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;

            return go.GetComponent<Button>();
        }
    }
}


