using UnityEngine;
using System.Collections.Generic;

namespace Dialogs
{
    /// <summary>
    /// Отладочный инструмент для системы диалогов
    /// </summary>
    public class DialogDebugger : MonoBehaviour
    {
        [Header("Отладка")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private string testDialogId = "interview_witness";

        private void OnEnable()
        {
            // Подписка на события диалогов
            DialogManager.OnDialogStarted += OnDialogStarted;
            DialogManager.OnDialogEnded += OnDialogEnded;
            DialogManager.OnNodePlayed += OnNodePlayed;
            
            // Подписка на события подсветки
            HighlightManager.OnHighlightsUpdated += OnHighlightsUpdated;
            HighlightManager.OnTooltipRequested += OnTooltipRequested;
        }

        private void OnDisable()
        {
            // Отписка от событий
            DialogManager.OnDialogStarted -= OnDialogStarted;
            DialogManager.OnDialogEnded -= OnDialogEnded;
            DialogManager.OnNodePlayed -= OnNodePlayed;
            
            // Отписка от событий подсветки
            HighlightManager.OnHighlightsUpdated -= OnHighlightsUpdated;
            HighlightManager.OnTooltipRequested -= OnTooltipRequested;
        }

        private void OnDialogStarted(Dialog dialog)
        {
            if (showDebugInfo)
            {
                Debug.Log($"🔵 Диалог начат: {dialog.id} (Говорящий: {dialog.speaker})");
            }
        }

        private void OnDialogEnded(Dialog dialog)
        {
            if (showDebugInfo)
            {
                Debug.Log($"🔴 Диалог завершен: {dialog.id}");
            }
        }

        private void OnNodePlayed(DialogNode node)
        {
            if (showDebugInfo)
            {
                Debug.Log($"💬 Узел проигран: {node.id}");
                Debug.Log($"   Текст: {node.text}");
                
                if (node.notebookEntries.Count > 0)
                {
                    Debug.Log($"   📝 Записи в блокнот: {node.notebookEntries.Count}");
                    foreach (var entry in node.notebookEntries)
                    {
                        Debug.Log($"      - Улика '{entry.clueId}': {entry.description}");
                    }
                }

                if (node.highlights.Count > 0)
                {
                    Debug.Log($"   🔍 Подсветки: {node.highlights.Count}");
                    foreach (var highlight in node.highlights)
                    {
                        Debug.Log($"      '{highlight.word}': {highlight.tooltips.Count} тултипов");
                    }
                }

                var availableOptions = DialogManager.Instance.GetAvailableOptions();
                Debug.Log($"   ✅ Доступных вариантов: {availableOptions.Count}");
                for (int i = 0; i < availableOptions.Count; i++)
                {
                    Debug.Log($"      {i}: {availableOptions[i].text}");
                }
            }
        }

        private void OnHighlightsUpdated(DialogNode node)
        {
            if (showDebugInfo && node != null)
            {
                Debug.Log($"🔍 Подсветки обновлены для узла: {node.id}");
                if (HighlightManager.Instance != null)
                {
                    var tooltips = HighlightManager.Instance.GetCurrentTooltips();
                    foreach (var kvp in tooltips)
                    {
                        Debug.Log($"   '{kvp.Key}': {kvp.Value.Count} доступных тултипов");
                    }
                }
            }
        }

        private void OnTooltipRequested(string word, List<Tooltip> tooltips)
        {
            if (showDebugInfo)
            {
                Debug.Log($"💬 Запрошен тултип для '{word}':");
                foreach (var tooltip in tooltips)
                {
                    Debug.Log($"   - {tooltip.text}");
                }
            }
        }

        [ContextMenu("Тест: Начать диалог")]
        public void TestStartDialog()
        {
            if (DialogManager.Instance != null)
            {
                DialogManager.Instance.StartDialog(testDialogId);
            }
            else
            {
                Debug.LogError("DialogManager не найден!");
            }
        }

        [ContextMenu("Тест: Выбрать вариант 0")]
        public void TestSelectOption0()
        {
            if (DialogManager.Instance != null && DialogManager.Instance.IsInDialog)
            {
                DialogManager.Instance.SelectOption(0);
            }
            else
            {
                Debug.LogWarning("Нет активного диалога для выбора варианта");
            }
        }

        [ContextMenu("Тест: Выбрать вариант 1")]
        public void TestSelectOption1()
        {
            if (DialogManager.Instance != null && DialogManager.Instance.IsInDialog)
            {
                DialogManager.Instance.SelectOption(1);
            }
            else
            {
                Debug.LogWarning("Нет активного диалога для выбора варианта");
            }
        }

        [ContextMenu("Тест: Завершить диалог")]
        public void TestEndDialog()
        {
            if (DialogManager.Instance != null && DialogManager.Instance.IsInDialog)
            {
                DialogManager.Instance.EndDialog();
            }
            else
            {
                Debug.LogWarning("Нет активного диалога для завершения");
            }
        }

        [ContextMenu("Тест: Показать все диалоги")]
        public void TestShowAllDialogs()
        {
            if (DialogManager.Instance != null)
            {
                var allDialogs = DialogManager.Instance.GetAllDialogs();
                Debug.Log($"📚 Всего загружено диалогов: {allDialogs.Count}");
                
                foreach (var dialog in allDialogs.Values)
                {
                    Debug.Log($"   - {dialog.id} (Говорящий: {dialog.speaker}, Узлов: {dialog.nodes.Count})");
                }
            }
            else
            {
                Debug.LogError("DialogManager не найден!");
            }
        }

        [ContextMenu("Тест: Перезагрузить диалоги")]
        public void TestReloadDialogs()
        {
            if (DialogManager.Instance != null)
            {
                DialogManager.Instance.LoadDialogs();
                Debug.Log("🔄 Диалоги перезагружены");
            }
            else
            {
                Debug.LogError("DialogManager не найден!");
            }
        }

        [ContextMenu("Тест: Показать подсветки")]
        public void TestShowHighlights()
        {
            if (HighlightManager.Instance != null)
            {
                var tooltips = HighlightManager.Instance.GetCurrentTooltips();
                Debug.Log($"🔍 Текущие подсветки: {tooltips.Count}");
                
                foreach (var kvp in tooltips)
                {
                    Debug.Log($"   '{kvp.Key}': {kvp.Value.Count} тултипов");
                    foreach (var tooltip in kvp.Value)
                    {
                        Debug.Log($"      - {tooltip.text}");
                    }
                }
            }
            else
            {
                Debug.LogError("HighlightManager не найден!");
            }
        }

        [ContextMenu("Тест: Запросить тултип для 'нож'")]
        public void TestRequestTooltipKnife()
        {
            if (HighlightManager.Instance != null)
            {
                HighlightManager.Instance.RequestTooltips("нож");
            }
            else
            {
                Debug.LogError("HighlightManager не найден!");
            }
        }

        [ContextMenu("Тест: Скрыть тултипы")]
        public void TestHideTooltips()
        {
            if (HighlightManager.Instance != null)
            {
                HighlightManager.Instance.HideTooltips();
                Debug.Log("🔍 Тултипы скрыты");
            }
            else
            {
                Debug.LogError("HighlightManager не найден!");
            }
        }

        // Отключено для релиза
        /*private void Update()
        {
            // Горячие клавиши для отладки
            if (Input.GetKeyDown(KeyCode.F1))
            {
                TestStartDialog();
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                TestSelectOption0();
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                TestSelectOption1();
            }
            else if (Input.GetKeyDown(KeyCode.F4))
            {
                TestEndDialog();
            }
            else if (Input.GetKeyDown(KeyCode.F5))
            {
                TestShowAllDialogs();
            }
            else if (Input.GetKeyDown(KeyCode.F6))
            {
                TestShowHighlights();
            }
            else if (Input.GetKeyDown(KeyCode.F7))
            {
                TestRequestTooltipKnife();
            }
            else if (Input.GetKeyDown(KeyCode.F8))
            {
                TestHideTooltips();
            }
        }*/
    }
}
