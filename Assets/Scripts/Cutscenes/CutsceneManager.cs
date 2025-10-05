using System;
using System.Collections.Generic;
using UnityEngine;
using Dialogs;

namespace Cutscenes
{
    /// <summary>
    /// Менеджер системы катсцен
    /// </summary>
    public class CutsceneManager : MonoBehaviour
    {
        public static CutsceneManager Instance { get; private set; }

        [Header("Настройки катсцен")]
        [SerializeField] private List<CutsceneData> cutscenes = new List<CutsceneData>();

        [Header("Настройки затемнения")]
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.5f;

        // Текущая катсцена
        private CutsceneData currentCutscene;
        private bool isInCutscene = false;

        // События
        public static event Action<CutsceneData> OnCutsceneStarted;
        public static event Action<CutsceneData> OnCutsceneEnded;

        // Свойства
        public bool IsInCutscene => isInCutscene;
        public CutsceneData CurrentCutscene => currentCutscene;

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
            DialogManager.OnDialogEnded += HandleDialogEnded;
        }

        private void OnDisable()
        {
            DialogManager.OnDialogEnded -= HandleDialogEnded;
        }

        /// <summary>
        /// Запустить катсцену по ID
        /// </summary>
        public void StartCutscene(string id)
        {
            if (isInCutscene)
            {
                Debug.LogWarning("Катсцена уже активна");
                return;
            }

            var cutscene = cutscenes.Find(c => c.id == id);
            if (cutscene == null)
            {
                Debug.LogError($"Катсцена с ID '{id}' не найдена!");
                return;
            }

            // Проверить наличие ScreenFader
            if (ScreenFader.Instance == null)
            {
                Debug.LogWarning("[CutsceneManager] ScreenFader не найден! Создайте GameObject с компонентом ScreenFader на сцене для затемнения экрана.");
            }

            currentCutscene = cutscene;
            isInCutscene = true;

            Debug.Log($"[CutsceneManager] Запуск катсцены: {id}");

            // Затемнить экран -> показать катсцену -> запустить диалог -> осветлить
            if (ScreenFader.Instance != null)
            {
                ScreenFader.Instance.FadeOut(fadeOutDuration, OnFadeOutComplete);
            }
            else
            {
                OnFadeOutComplete();
            }
        }

        private void OnFadeOutComplete()
        {
            // Уведомить UI о начале катсцены
            OnCutsceneStarted?.Invoke(currentCutscene);

            // Запустить связанный диалог
            if (!string.IsNullOrEmpty(currentCutscene.dialogId))
            {
                if (DialogManager.Instance != null)
                {
                    DialogManager.Instance.StartDialog(currentCutscene.dialogId);
                }
                else
                {
                    Debug.LogError("DialogManager не найден!");
                    EndCutscene();
                    return;
                }
            }

            // Осветлить экран
            if (ScreenFader.Instance != null)
            {
                ScreenFader.Instance.FadeIn(fadeInDuration);
            }
        }

        /// <summary>
        /// Обработчик завершения диалога
        /// </summary>
        private void HandleDialogEnded(Dialog dialog)
        {
            // Если катсцена активна и диалог принадлежит ей - завершить катсцену
            if (isInCutscene && currentCutscene != null && currentCutscene.dialogId == dialog.id)
            {
                EndCutscene();
            }
        }

        /// <summary>
        /// Завершить текущую катсцену
        /// </summary>
        public void EndCutscene()
        {
            if (!isInCutscene) return;

            // Затемнить -> скрыть катсцену -> осветлить
            if (ScreenFader.Instance != null)
            {
                ScreenFader.Instance.FadeOut(fadeOutDuration, OnEndFadeOutComplete);
            }
            else
            {
                OnEndFadeOutComplete();
            }
        }

        private void OnEndFadeOutComplete()
        {
            var cutsceneToEnd = currentCutscene;
            currentCutscene = null;
            isInCutscene = false;

            // Уведомить UI о завершении
            OnCutsceneEnded?.Invoke(cutsceneToEnd);

            // Осветлить экран
            if (ScreenFader.Instance != null)
            {
                ScreenFader.Instance.FadeIn(fadeInDuration);
            }
        }

        /// <summary>
        /// Получить катсцену по ID
        /// </summary>
        public CutsceneData GetCutscene(string id)
        {
            return cutscenes.Find(c => c.id == id);
        }

        /// <summary>
        /// Добавить катсцену в список
        /// </summary>
        public void AddCutscene(CutsceneData cutscene)
        {
            if (cutscenes.Find(c => c.id == cutscene.id) != null)
            {
                Debug.LogWarning($"Катсцена с ID '{cutscene.id}' уже существует");
                return;
            }
            cutscenes.Add(cutscene);
        }
    }
}

