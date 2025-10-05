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
        [SerializeField] private GameModeManager gameModeManager;

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
            gameModeManager.SwitchMode(GameMode.Dialogue);
        }

        private void HandleEnded(Dialog dialog)
        {
            if (dialogPanel != null) dialogPanel.SetActive(false);
            gameModeManager.SwitchMode(GameMode.Play);
        }
    }
}