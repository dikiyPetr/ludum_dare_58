using UnityEngine;

namespace Tutorial
{
    /// <summary>
    /// Компонент для управления подсветкой объекта в системе обучения
    /// </summary>
    [RequireComponent(typeof(Outline))]
    public class GuidanceHighlight : MonoBehaviour
    {
        [Header("Настройки подсветки")]
        [SerializeField] private Color guidanceColor = Color.yellow;
        [SerializeField] private float guidanceWidth = 3f;
        [SerializeField] private bool usePulsingEffect = true;
        [SerializeField] private float pulseSpeed = 2f;

        [Header("Триггер приближения")]
        [SerializeField] private float triggerDistance = 3f;
        [SerializeField] private string playerTag = "Player";

        private Outline outline;
        private Color originalColor;
        private float originalWidth;
        private bool isHighlighted = false;
        private bool isCompleted = false;
        private Transform playerTransform;

        private void Awake()
        {
            outline = GetComponent<Outline>();
            if (outline == null)
            {
                Debug.LogError($"[GuidanceHighlight] На объекте {name} не найден компонент Outline!");
                return;
            }

            // Сохраняем оригинальные настройки
            originalColor = outline.OutlineColor;
            originalWidth = outline.OutlineWidth;
        }

        private void Start()
        {
            // Находим игрока
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning($"[GuidanceHighlight] Игрок с тегом '{playerTag}' не найден!");
            }

            // Изначально отключаем обводку
            outline.enabled = false;
        }

        private void Update()
        {
            if (isHighlighted && !isCompleted && usePulsingEffect)
            {
                UpdatePulsingEffect();
            }

            if (isHighlighted && !isCompleted && playerTransform != null)
            {
                CheckPlayerDistance();
            }
        }

        /// <summary>
        /// Начать подсветку объекта
        /// </summary>
        public void StartHighlight()
        {
            if (isCompleted || outline == null) return;

            isHighlighted = true;
            ForceEnableOutline();

            Debug.Log($"[GuidanceHighlight] Начата подсветка объекта: {name}");
        }

        /// <summary>
        /// Остановить подсветку объекта
        /// </summary>
        public void StopHighlight()
        {
            if (outline == null) return;

            isHighlighted = false;
            ForceDisableOutline();

            Debug.Log($"[GuidanceHighlight] Остановлена подсветка объекта: {name}");
        }

        /// <summary>
        /// Завершить обучение для этого объекта (навсегда)
        /// </summary>
        public void CompleteGuidance()
        {
            if (isCompleted) return;

            isCompleted = true;
            StopHighlight();

            // Уведомляем менеджер о завершении
            if (GuidanceManager.Instance != null)
            {
                GuidanceManager.Instance.OnGuidanceCompleted(this);
            }

            Debug.Log($"[GuidanceHighlight] Обучение завершено для объекта: {name}");
        }

        /// <summary>
        /// Обновление пульсирующего эффекта
        /// </summary>
        private void UpdatePulsingEffect()
        {
            if (outline == null) return;

            float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;
            float currentWidth = guidanceWidth + (guidanceWidth * 0.3f * pulse);
            outline.OutlineWidth = currentWidth;
        }

        /// <summary>
        /// Проверка расстояния до игрока
        /// </summary>
        private void CheckPlayerDistance()
        {
            if (playerTransform == null) return;

            float distance = Vector3.Distance(transform.position, playerTransform.position);
            
            if (distance <= triggerDistance)
            {
                CompleteGuidance();
            }
        }

        /// <summary>
        /// Установить расстояние срабатывания триггера
        /// </summary>
        public void SetTriggerDistance(float distance)
        {
            triggerDistance = distance;
        }

        /// <summary>
        /// Установить цвет подсветки
        /// </summary>
        public void SetGuidanceColor(Color color)
        {
            guidanceColor = color;
            if (isHighlighted && outline != null)
            {
                outline.OutlineColor = guidanceColor;
            }
        }

        /// <summary>
        /// Установить ширину обводки
        /// </summary>
        public void SetGuidanceWidth(float width)
        {
            guidanceWidth = width;
            if (isHighlighted && outline != null)
            {
                outline.OutlineWidth = guidanceWidth;
            }
        }

        /// <summary>
        /// Проверить завершено ли обучение для этого объекта
        /// </summary>
        public bool IsCompleted()
        {
            return isCompleted;
        }

        /// <summary>
        /// Проверить активна ли подсветка
        /// </summary>
        public bool IsHighlighted()
        {
            return isHighlighted;
        }

        /// <summary>
        /// Принудительно включить обводку (игнорирует OutlineSelector)
        /// </summary>
        private void ForceEnableOutline()
        {
            if (outline == null) return;

            outline.enabled = true;
            outline.OutlineColor = guidanceColor;
            outline.OutlineWidth = guidanceWidth;
        }

        /// <summary>
        /// Принудительно выключить обводку (игнорирует OutlineSelector)
        /// </summary>
        private void ForceDisableOutline()
        {
            if (outline == null) return;

            outline.enabled = false;
            outline.OutlineColor = originalColor;
            outline.OutlineWidth = originalWidth;
        }

        private void OnDrawGizmosSelected()
        {
            // Рисуем радиус триггера в редакторе
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, triggerDistance);
        }
    }
}
