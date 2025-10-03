using UnityEngine;

// ========== КОМПОНЕНТ ТРИГГЕРА ==========
[RequireComponent(typeof(Collider))]
public class SoundTrigger : MonoBehaviour
{
    [Header("Настройки триггера")] [Tooltip("Тег объекта, который активирует триггер (например, Player)")]
    public string targetTag = "Player";

    [Header("Связанный Condition")] [Tooltip("Перетащите сюда компонент SoundCondition")]
    public SoundCondition soundCondition;

    private void Start()
    {
        // Проверяем, что коллайдер настроен как триггер
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"Коллайдер на {gameObject.name} не настроен как триггер! Включаю автоматически.");
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && soundCondition != null)
        {
            soundCondition.OnTriggerEntered(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(targetTag) && soundCondition != null)
        {
            soundCondition.OnTriggerStaying(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag) && soundCondition != null)
        {
            soundCondition.OnTriggerExited(other.gameObject);
        }
    }
}