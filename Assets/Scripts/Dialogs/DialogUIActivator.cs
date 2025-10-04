using UnityEngine;

namespace Dialogs
{
    /// <summary>
    /// Активирует UI-панель диалогов при старте диалога и скрывает при окончании
    /// Подключается к событиям DialogManager и включает/выключает объект панели на сцене
    /// </summary>
    public class DialogUIActivator : MonoBehaviour
    {
        [SerializeField] private GameObject dialogPanel; // UI панель (например, Overlay/DialogPanel)

        void Awake()
        {
            if (dialogPanel != null)
            {
                dialogPanel.SetActive(false);
            }
        }

        void OnEnable()
        {
            DialogManager.OnDialogStarted += HandleStarted;
            DialogManager.OnDialogEnded += HandleEnded;
        }

        void OnDisable()
        {
            DialogManager.OnDialogStarted -= HandleStarted;
            DialogManager.OnDialogEnded -= HandleEnded;
        }

        private void HandleStarted(Dialog dialog)
        {
            if (dialogPanel != null) dialogPanel.SetActive(true);
            // Блокируем управление персонажем на время диалога (если есть контроллер)
            var controller = FindObjectOfType<SimpleCharacterController>();
            if (controller != null) controller.DisableController();
        }

        private void HandleEnded(Dialog dialog)
        {
            if (dialogPanel != null) dialogPanel.SetActive(false);
            var controller = FindObjectOfType<SimpleCharacterController>();
            if (controller != null) controller.EnableController();
        }
    }
}


