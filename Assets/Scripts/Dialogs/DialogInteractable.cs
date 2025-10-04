using UnityEngine;

namespace Dialogs
{
    /// <summary>
    /// Простой интерактор: при нахождении игрока в триггере и нажатии E запускает диалог
    /// </summary>
    public class DialogInteractable : MonoBehaviour
    {
        [Header("Диалог")]
        [SerializeField] private string dialogId = "interview_witness";
        [SerializeField] private string playerTag = "Player";

        private bool isPlayerInside;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                isPlayerInside = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                isPlayerInside = false;
            }
        }

        void Update()
        {
            if (!isPlayerInside) return;

            // Используем стандартный KeyCode.E для простоты интеграции
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (DialogManager.Instance != null && !DialogManager.Instance.IsInDialog)
                {
                    DialogManager.Instance.StartDialog(dialogId);
                }
            }
        }
    }
}


