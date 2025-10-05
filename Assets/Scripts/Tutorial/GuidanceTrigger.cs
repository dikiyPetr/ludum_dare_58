using UnityEngine;

namespace Tutorial
{
    /// <summary>
    /// Триггер для отслеживания приближения игрока к объекту обучения
    /// Альтернатива к проверке расстояния в GuidanceHighlight
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class GuidanceTrigger : MonoBehaviour
    {
        [Header("Настройки триггера")]
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private bool triggerOnce = true;
        [SerializeField] private bool showDebugInfo = true;

        [Header("Связанные компоненты")]
        [SerializeField] private GuidanceHighlight guidanceHighlight;

        private bool hasTriggered = false;
        private Collider triggerCollider;

        private void Awake()
        {
            triggerCollider = GetComponent<Collider>();
            
            // Убеждаемся что коллайдер настроен как триггер
            if (!triggerCollider.isTrigger)
            {
                Debug.LogWarning($"[GuidanceTrigger] Коллайдер на {gameObject.name} не настроен как триггер! Включаю автоматически.");
                triggerCollider.isTrigger = true;
            }

            // Если GuidanceHighlight не назначен, пытаемся найти его на том же объекте
            if (guidanceHighlight == null)
            {
                guidanceHighlight = GetComponent<GuidanceHighlight>();
            }

            // Если все еще не найден, ищем в родительских объектах
            if (guidanceHighlight == null)
            {
                guidanceHighlight = GetComponentInParent<GuidanceHighlight>();
            }

            if (guidanceHighlight == null)
            {
                Debug.LogError($"[GuidanceTrigger] На объекте {name} не найден компонент GuidanceHighlight!");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;
            if (hasTriggered && triggerOnce) return;
            if (guidanceHighlight == null) return;

            if (showDebugInfo)
            {
                Debug.Log($"[GuidanceTrigger] Игрок вошел в зону триггера: {name}");
            }

            // Завершаем обучение для этого объекта
            guidanceHighlight.CompleteGuidance();
            
            if (triggerOnce)
            {
                hasTriggered = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;
            
            if (showDebugInfo)
            {
                Debug.Log($"[GuidanceTrigger] Игрок вышел из зоны триггера: {name}");
            }
        }

        /// <summary>
        /// Установить связанный GuidanceHighlight
        /// </summary>
        public void SetGuidanceHighlight(GuidanceHighlight highlight)
        {
            guidanceHighlight = highlight;
        }

        /// <summary>
        /// Сбросить состояние триггера (позволяет сработать снова)
        /// </summary>
        public void ResetTrigger()
        {
            hasTriggered = false;
        }

        /// <summary>
        /// Проверить сработал ли триггер
        /// </summary>
        public bool HasTriggered()
        {
            return hasTriggered;
        }

        private void OnDrawGizmosSelected()
        {
            // Рисуем границы триггера
            if (triggerCollider != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.matrix = transform.localToWorldMatrix;
                
                if (triggerCollider is BoxCollider boxCollider)
                {
                    Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
                }
                else if (triggerCollider is SphereCollider sphereCollider)
                {
                    Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
                }
                else if (triggerCollider is CapsuleCollider capsuleCollider)
                {
                    Gizmos.DrawWireCube(capsuleCollider.center, new Vector3(capsuleCollider.radius * 2, capsuleCollider.height, capsuleCollider.radius * 2));
                }
            }
        }
    }
}
