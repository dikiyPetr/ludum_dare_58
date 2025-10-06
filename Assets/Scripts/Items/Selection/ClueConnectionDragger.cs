using UnityEngine;
using UnityEngine.InputSystem;

public class ClueConnectionDragger : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private ColliderOutlineSelector outlineSelector;
    [SerializeField] private float rayDistance = 2f;
    [SerializeField] private InputActionReference clickAction;
    [SerializeField] private ClueConnectionRenderer connectionRenderer;
    [SerializeField] private float updateThrottleInterval = 0.05f; // Интервал между обновлениями линии в секундах

    private CabinetSlotInteractable firstSelectedSlot = null;
    private Outline currentOutline = null;
    private bool isDragging = false;
    private const string TEMP_LINE_KEY = "temp_connection";
    private float lastUpdateTime = 0f;

    private void OnEnable()
    {
        if (clickAction != null)
        {
            clickAction.action.Enable();
            clickAction.action.started += OnClickStarted;
            clickAction.action.performed += OnClickPerformed;
            clickAction.action.canceled += OnClickCanceled;
        }
    }

    private void OnDisable()
    {
        if (clickAction != null)
        {
            clickAction.action.started -= OnClickStarted;
            clickAction.action.performed -= OnClickPerformed;
            clickAction.action.canceled -= OnClickCanceled;
            clickAction.action.Disable();
        }
    }

    void Update()
    {
        // Если мы в режиме перетаскивания, обновляем линию
        if (isDragging && firstSelectedSlot != null && connectionRenderer != null)
        {
            // Проверяем тротлинг
            if (Time.time - lastUpdateTime < updateThrottleInterval)
            {
                return;
            }
            lastUpdateTime = Time.time;

            Vector3 targetPoint = GetTargetPoint();
            UpdateDragLine(targetPoint);
        }
    }

    private Vector3 GetTargetPoint()
    {
        if (outlineSelector == null || mainCamera == null)
        {
            // Фоллбэк: точка на луче на расстоянии от камеры
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            return ray.GetPoint(rayDistance);
        }

        Outline targetOutline = outlineSelector.GetCurrentOutline();

        if (targetOutline != null)
        {
            // Используем позицию объекта из ColliderOutlineSelector
            return targetOutline.transform.position;
        }
        else
        {
            // Если outline пустой, используем позицию самого ColliderOutlineSelector
            return outlineSelector.transform.position;
        }
    }

    private void OnClickStarted(InputAction.CallbackContext context)
    {
        Debug.Log($"OnClickStarted");

        if (outlineSelector == null) return;

        // Получаем текущий outline из ColliderOutlineSelector
        currentOutline = outlineSelector.GetCurrentOutline();

        if (currentOutline == null) return;

        CabinetSlotInteractable slotInteractable = currentOutline.GetComponent<CabinetSlotInteractable>();
        if (slotInteractable == null || slotInteractable.GetClue() == null) return;

        // Выбираем первую улику и начинаем перетаскивание
        firstSelectedSlot = slotInteractable;
        isDragging = true;
        Debug.Log($"<color=cyan>[ClueConnectionDragger]</color> Начало перетаскивания от улики: {slotInteractable.GetClue().id}");
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        // Можно использовать для дополнительной логики при достижении Press Point
        Debug.Log($"<color=magenta>[ClueConnectionDragger]</color> Click performed (нажатие зафиксировано)");
    }

    private void OnClickCanceled(InputAction.CallbackContext context)
    {
        Debug.Log($"OnClickCanceled )");
        if (!isDragging) return;

        // Удаляем временную линию
        if (connectionRenderer != null)
        {
            connectionRenderer.RemoveConnectionLine(TEMP_LINE_KEY);
        }

        // Проверяем, на что отпустили
        if (outlineSelector != null)
        {
            Outline endOutline = outlineSelector.GetCurrentOutline();

            if (endOutline != null)
            {
                CabinetSlotInteractable slotInteractable = endOutline.GetComponent<CabinetSlotInteractable>();
                if (slotInteractable != null && slotInteractable.GetClue() != null && firstSelectedSlot != null && slotInteractable != firstSelectedSlot)
                {
                    string clueId1 = firstSelectedSlot.GetClue().id;
                    string clueId2 = slotInteractable.GetClue().id;

                    Debug.Log($"<color=cyan>[ClueConnectionDragger]</color> Проверка связи между '{clueId1}' и '{clueId2}'...");

                    bool success = ClueManager.Instance.TryDiscoverConnection(clueId1, clueId2);

                    if (success)
                    {
                        Debug.Log($"<color=green>[ClueConnectionDragger]</color> ✓ Связь обнаружена!");
                    }
                    else
                    {
                        Debug.Log($"<color=red>[ClueConnectionDragger]</color> ✗ Связь не найдена или уже известна");
                    }
                }
            }
        }

        // Сбрасываем состояние
        isDragging = false;
        firstSelectedSlot = null;
        currentOutline = null;
    }

    private void UpdateDragLine(Vector3 targetPoint)
    {
        if (firstSelectedSlot == null || connectionRenderer == null) return;

        Vector3 startPos = firstSelectedSlot.transform.position;

        // Создаем или обновляем временную линию
        if (connectionRenderer.HasLine(TEMP_LINE_KEY))
        {
            connectionRenderer.UpdateConnectionLine(TEMP_LINE_KEY, startPos, targetPoint);
        }
        else
        {
            connectionRenderer.CreateConnectionLine(TEMP_LINE_KEY, startPos, targetPoint);
        }
    }
}