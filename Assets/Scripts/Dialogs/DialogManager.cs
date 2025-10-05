using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Dialogs
{
    /// <summary>
    /// Менеджер системы диалогов
    /// </summary>
    public class DialogManager : MonoBehaviour
    {
        public static DialogManager Instance { get; private set; }

        [Header("Настройки")]
        [SerializeField] private string dialogsJsonPath = "dialogs.json";
        [SerializeField] private bool loadOnStart = true;

        // События
        public static event Action<DialogNode> OnNodePlayed;
        public static event Action<Dialog> OnDialogStarted;
        public static event Action<Dialog> OnDialogEnded;
        public static event Action<DialogNode> OnHighlightsUpdated;
        public static event Action<Dialog, DialogNode, DialogOption> OnOptionSelected;

        // Данные
        private Dictionary<string, Dialog> dialogs = new Dictionary<string, Dialog>();
        private Dialog currentDialog;
        private DialogNode currentNode;

        // Состояние
        public bool IsInDialog => currentDialog != null;
        public Dialog CurrentDialog => currentDialog;
        public DialogNode CurrentNode => currentNode;

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

        private void Start()
        {
            if (loadOnStart)
            {
                LoadDialogs();
            }
        }

        /// <summary>
        /// Загрузить диалоги из JSON файла
        /// </summary>
        public void LoadDialogs()
        {
            try
            {
                string filePath = Path.Combine(Application.streamingAssetsPath, dialogsJsonPath);
                
                if (!File.Exists(filePath))
                {
                    Debug.LogError($"Файл диалогов не найден: {filePath}");
                    return;
                }

                string jsonContent = File.ReadAllText(filePath);
                
                // Используем кастомный парсер для поддержки полиморфных условий
                var dialogsList = DialogJsonParser.ParseDialogs(jsonContent);

                if (dialogsList != null && dialogsList.Count > 0)
                {
                    dialogs.Clear();
                    foreach (var dialog in dialogsList)
                    {
                        dialogs[dialog.id] = dialog;
                    }

                    Debug.Log($"Загружено {dialogs.Count} диалогов");
                }
                else
                {
                    Debug.LogError("Ошибка парсинга JSON файла диалогов");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка загрузки диалогов: {e.Message}");
            }
        }

        /// <summary>
        /// Начать диалог по ID
        /// </summary>
        public bool StartDialog(string dialogId)
        {
            if (IsInDialog)
            {
                Debug.LogWarning("Диалог уже активен. Завершите текущий диалог перед началом нового.");
                return false;
            }

            if (!dialogs.TryGetValue(dialogId, out currentDialog))
            {
                Debug.LogError($"Диалог с ID '{dialogId}' не найден!");
                return false;
            }

            // Найти стартовый узел
            currentNode = FindNodeById(currentDialog.startNodeId);
            if (currentNode == null)
            {
                Debug.LogError($"Стартовый узел '{currentDialog.startNodeId}' не найден в диалоге '{dialogId}'!");
                currentDialog = null;
                return false;
            }

            Debug.Log($"Начат диалог: {dialogId}");
            OnDialogStarted?.Invoke(currentDialog);
            
            // Проиграть стартовый узел
            PlayCurrentNode();
            
            return true;
        }

        /// <summary>
        /// Выбрать вариант ответа
        /// </summary>
        public bool SelectOption(int optionIndex)
        {
            if (!IsInDialog || currentNode == null)
            {
                Debug.LogWarning("Нет активного диалога для выбора варианта");
                return false;
            }

            if (optionIndex < 0 || optionIndex >= currentNode.options.Count)
            {
                Debug.LogError($"Неверный индекс варианта: {optionIndex}");
                return false;
            }

            var option = currentNode.options[optionIndex];
            
            // Проверить условие доступности
            if (!ConditionEvaluator.Evaluate(option.condition))
            {
                Debug.LogWarning($"Вариант '{option.text}' недоступен из-за невыполненного условия");
                return false;
            }

            // Уведомить о выборе опции
            OnOptionSelected?.Invoke(currentDialog, currentNode, option);

            // Перейти к следующему узлу
            if (string.IsNullOrEmpty(option.nextNodeId))
            {
                // Диалог завершен
                EndDialog();
            }
            else
            {
                // Перейти к следующему узлу
                var nextNode = FindNodeById(option.nextNodeId);
                if (nextNode == null)
                {
                    Debug.LogError($"Следующий узел '{option.nextNodeId}' не найден!");
                    EndDialog();
                    return false;
                }

                currentNode = nextNode;
                PlayCurrentNode();
            }

            return true;
        }

        /// <summary>
        /// Завершить текущий диалог
        /// </summary>
        public void EndDialog()
        {
            if (!IsInDialog) return;

            var dialogToEnd = currentDialog;
            currentDialog = null;
            currentNode = null;

            Debug.Log($"Диалог завершен: {dialogToEnd.id}");
            OnDialogEnded?.Invoke(dialogToEnd);
        }

        /// <summary>
        /// Проиграть текущий узел
        /// </summary>
        private void PlayCurrentNode()
        {
            if (currentNode == null) return;

            // Обработать записи в блокнот (добавление улик)
            ProcessNotebookEntries();

            // Уведомить внешние системы о проигрывании узла
            OnNodePlayed?.Invoke(currentNode);

            // Уведомить о подсветках
            OnHighlightsUpdated?.Invoke(currentNode);

            Debug.Log($"[{currentDialog.speaker}]: {currentNode.text}");
        }

        /// <summary>
        /// Обработать записи в блокнот - добавить улики
        /// </summary>
        private void ProcessNotebookEntries()
        {
            if (currentNode?.notebookEntries == null || currentNode.notebookEntries.Count == 0)
                return;

            if (ClueManager.Instance == null)
            {
                Debug.LogWarning("ClueManager не найден, невозможно добавить улики из notebookEntries");
                return;
            }

            foreach (var entry in currentNode.notebookEntries)
            {
                if (string.IsNullOrEmpty(entry.clueId))
                {
                    Debug.LogWarning($"NotebookEntry имеет пустой clueId в узле '{currentNode.id}'");
                    continue;
                }

                // Добавить улику через ClueManager
                ClueManager.Instance.AddClue(entry.clueId);
            }
        }

        /// <summary>
        /// Найти узел по ID в текущем диалоге
        /// </summary>
        private DialogNode FindNodeById(string nodeId)
        {
            if (currentDialog == null) return null;

            foreach (var node in currentDialog.nodes)
            {
                if (node.id == nodeId)
                    return node;
            }

            return null;
        }

        /// <summary>
        /// Получить доступные варианты ответов для текущего узла
        /// </summary>
        public List<DialogOption> GetAvailableOptions()
        {
            var availableOptions = new List<DialogOption>();

            if (currentNode == null) return availableOptions;

            foreach (var option in currentNode.options)
            {
                if (ConditionEvaluator.Evaluate(option.condition))
                {
                    availableOptions.Add(option);
                }
            }

            return availableOptions;
        }

        /// <summary>
        /// Получить диалог по ID
        /// </summary>
        public Dialog GetDialog(string dialogId)
        {
            dialogs.TryGetValue(dialogId, out var dialog);
            return dialog;
        }

        /// <summary>
        /// Получить все загруженные диалоги
        /// </summary>
        public Dictionary<string, Dialog> GetAllDialogs()
        {
            return new Dictionary<string, Dialog>(dialogs);
        }

        /// <summary>
        /// Проверить, загружен ли диалог
        /// </summary>
        public bool IsDialogLoaded(string dialogId)
        {
            return dialogs.ContainsKey(dialogId);
        }
    }
}
