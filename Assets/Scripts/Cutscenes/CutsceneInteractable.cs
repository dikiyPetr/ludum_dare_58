using UnityEngine;
using UnityEngine.InputSystem;

namespace Cutscenes
{
    /// <summary>
    /// Интерактор для запуска катсцены при нажатии ЛКМ
    /// </summary>
    public class CutsceneInteractable : MonoBehaviour
    {
        [Header("Катсцена")]
        [SerializeField] private string cutsceneId = "";
        [SerializeField] private string playerTag = "Player";
        
        [Header("Настройки")]
        [SerializeField] private bool playOnce = true;

        private bool isPlayerInside;
        private bool hasPlayed = false;
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

            // Поддержка старого и нового ввода - ЛКМ
            // if (Input.GetMouseButtonDown(0) || (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame))
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                TryStartCutscene();
            }
        }

        private void OnInteract(InputAction.CallbackContext ctx)
        {
            if (!isPlayerInside) return;
            TryStartCutscene();
        }

        private void TryStartCutscene()
        {
            // Проверка на повторный запуск
            if (playOnce && hasPlayed)
            {
                return;
            }

            if (CutsceneManager.Instance != null && !CutsceneManager.Instance.IsInCutscene)
            {
                CutsceneManager.Instance.StartCutscene(cutsceneId);
                hasPlayed = true;
            }
        }

        /// <summary>
        /// Сбросить флаг проигрывания
        /// </summary>
        public void ResetPlayedFlag()
        {
            hasPlayed = false;
        }

        void OnDrawGizmosSelected()
        {
            var collider = GetComponent<BoxCollider>();
            if (collider != null)
            {
                Gizmos.color = new Color(0f, 0.8f, 1f, 0.25f);
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(collider.center, collider.size);
                Gizmos.color = new Color(0f, 0.8f, 1f, 1f);
                Gizmos.DrawWireCube(collider.center, collider.size);
            }
        }
    }
}

