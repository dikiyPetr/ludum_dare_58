using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
    /// <summary>
    /// Менеджер системы обучения - управляет подсветкой объектов по триггерам
    /// </summary>
    public class GuidanceManager : MonoBehaviour
    {
        public static GuidanceManager Instance { get; private set; }

        [Header("Настройки")]
        [SerializeField] private bool enableGuidanceOnStart = true;
        [SerializeField] private float guidanceDelay = 1f;

        [Header("Объекты для подсветки")]
        [SerializeField] private List<GuidanceHighlight> guidanceObjects = new List<GuidanceHighlight>();

        private int currentGuidanceIndex = 0;
        private bool isGuidanceActive = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (enableGuidanceOnStart)
            {
                StartGuidance();
            }
        }

        /// <summary>
        /// Запустить систему обучения
        /// </summary>
        public void StartGuidance()
        {
            if (isGuidanceActive) return;

            isGuidanceActive = true;
            currentGuidanceIndex = 0;
            
            Debug.Log($"[GuidanceManager] Запуск системы обучения. Объектов для подсветки: {guidanceObjects.Count}");
            
            ShowNextGuidance();
        }

        /// <summary>
        /// Остановить систему обучения
        /// </summary>
        public void StopGuidance()
        {
            if (!isGuidanceActive) return;

            isGuidanceActive = false;
            
            // Отключаем все подсветки
            foreach (var guidance in guidanceObjects)
            {
                if (guidance != null)
                {
                    guidance.StopHighlight();
                }
            }
            
            Debug.Log("[GuidanceManager] Система обучения остановлена");
        }

        /// <summary>
        /// Показать следующий объект для подсветки
        /// </summary>
        public void ShowNextGuidance()
        {
            if (!isGuidanceActive || currentGuidanceIndex >= guidanceObjects.Count)
            {
                Debug.Log("[GuidanceManager] Обучение завершено");
                StopGuidance();
                return;
            }

            var currentGuidance = guidanceObjects[currentGuidanceIndex];
            if (currentGuidance != null)
            {
                Debug.Log($"[GuidanceManager] Подсветка объекта: {currentGuidance.name}");
                currentGuidance.StartHighlight();
            }
            else
            {
                Debug.LogWarning($"[GuidanceManager] Объект с индексом {currentGuidanceIndex} не найден!");
                currentGuidanceIndex++;
                ShowNextGuidance();
            }
        }

        /// <summary>
        /// Вызывается когда игрок приблизился к подсвеченному объекту
        /// </summary>
        public void OnGuidanceCompleted(GuidanceHighlight completedGuidance)
        {
            if (!isGuidanceActive) return;

            Debug.Log($"[GuidanceManager] Обучение завершено для объекта: {completedGuidance.name}");
            
            // Отключаем подсветку текущего объекта
            completedGuidance.StopHighlight();
            
            // Переходим к следующему
            currentGuidanceIndex++;
            
            // Показываем следующий объект с задержкой
            if (guidanceDelay > 0)
            {
                Invoke(nameof(ShowNextGuidance), guidanceDelay);
            }
            else
            {
                ShowNextGuidance();
            }
        }

        /// <summary>
        /// Добавить объект для обучения
        /// </summary>
        public void AddGuidanceObject(GuidanceHighlight guidanceObject)
        {
            if (guidanceObject != null && !guidanceObjects.Contains(guidanceObject))
            {
                guidanceObjects.Add(guidanceObject);
                Debug.Log($"[GuidanceManager] Добавлен объект для обучения: {guidanceObject.name}");
            }
        }

        /// <summary>
        /// Удалить объект из обучения
        /// </summary>
        public void RemoveGuidanceObject(GuidanceHighlight guidanceObject)
        {
            if (guidanceObjects.Remove(guidanceObject))
            {
                Debug.Log($"[GuidanceManager] Удален объект из обучения: {guidanceObject.name}");
            }
        }

        /// <summary>
        /// Получить текущий активный объект обучения
        /// </summary>
        public GuidanceHighlight GetCurrentGuidance()
        {
            if (currentGuidanceIndex < guidanceObjects.Count)
            {
                return guidanceObjects[currentGuidanceIndex];
            }
            return null;
        }

        /// <summary>
        /// Проверить активна ли система обучения
        /// </summary>
        public bool IsGuidanceActive()
        {
            return isGuidanceActive;
        }

        /// <summary>
        /// Проверить, является ли объект текущим активным объектом обучения
        /// </summary>
        public bool IsCurrentGuidanceObject(GuidanceHighlight guidanceObject)
        {
            if (!isGuidanceActive || currentGuidanceIndex >= guidanceObjects.Count)
                return false;

            return guidanceObjects[currentGuidanceIndex] == guidanceObject;
        }
    }
}
