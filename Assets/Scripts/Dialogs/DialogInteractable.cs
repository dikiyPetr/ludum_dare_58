using UnityEngine;
using UnityEngine.InputSystem;

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
        private InputSystem_Actions inputActions;

        void Awake()
        {
            inputActions = new InputSystem_Actions();
        }

        void OnEnable()
        {
            if (inputActions != null)
            {
                inputActions.Player.Enable();
                inputActions.Player.Interact.started += OnInteract;
                inputActions.Player.Interact.performed += OnInteract;
            }
        }

        void OnDisable()
        {
            if (inputActions != null)
            {
                inputActions.Player.Interact.started -= OnInteract;
                inputActions.Player.Interact.performed -= OnInteract;
                inputActions.Player.Disable();
            }
        }

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

        void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                isPlayerInside = true;
            }
        }

        void Update()
        {
            if (!isPlayerInside) return;

            // Поддержка старого и нового ввода: KeyCode.E и Keyboard.current.eKey
            if (Input.GetKeyDown(KeyCode.E) || (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame))
            {
                TryStartDialog();
            }
        }

        private void OnInteract(InputAction.CallbackContext ctx)
        {
            if (!isPlayerInside) return;
            TryStartDialog();
        }

        private void TryStartDialog()
        {
            if (DialogManager.Instance != null && !DialogManager.Instance.IsInDialog)
            {
                DialogManager.Instance.StartDialog(dialogId);
            }
        }

        void OnDrawGizmosSelected()
        {
            var collider = GetComponent<BoxCollider>();
            if (collider != null)
            {
                Gizmos.color = new Color(1f, 0.8f, 0f, 0.25f);
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(collider.center, collider.size);
                Gizmos.color = new Color(1f, 0.8f, 0f, 1f);
                Gizmos.DrawWireCube(collider.center, collider.size);
            }
        }
    }
}


