using UnityEngine;
using UnityEngine.InputSystem;

public class ClueConnectionDragger : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float rayDistance = 100f;
    [SerializeField] private InputActionReference clickAction;
    [SerializeField] private ClueConnectionRenderer connectionRenderer;

    private CabinetSlot firstSelectedSlot = null;
    private Outline currentOutline = null;
    private bool isDragging = false;
    private const string TEMP_LINE_KEY = "temp_connection";

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
            // Получаем центр экрана
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

            // Создаем луч из центра экрана
            Ray ray = mainCamera.ScreenPointToRay(screenCenter);
            RaycastHit hit;

            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                targetPoint = hit.point;
            }
            else
            {
                // Получаем точку на луче на расстоянии от камеры
                targetPoint = ray.GetPoint(rayDistance);
            }

            UpdateDragLine(targetPoint);
        }
    }

    private void OnClickStarted(InputAction.CallbackContext context)
    {
        Debug.Log($"OnClickStarted");

        // Получаем центр экрана
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        // Создаем луч из центра экрана
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        // Выполняем райкаст
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // Пытаемся получить компонент Outline
            currentOutline = hit.collider.GetComponent<Outline>();

            if (currentOutline == null) return;

            CabinetSlot slot = currentOutline.GetComponentInParent<CabinetSlot>();
            if (slot == null || slot.GetClue() == null) return;

            // Выбираем первую улику и начинаем перетаскивание
            firstSelectedSlot = slot;
            isDragging = true;
            Debug.Log($"<color=cyan>[ClueConnectionDragger]</color> Начало перетаскивания от улики: {slot.GetClue().id}");
        }
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

        // Получаем центр экрана
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        // Создаем луч из центра экрана
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        // Выполняем райкаст для проверки, на что отпустили
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            Outline endOutline = hit.collider.GetComponent<Outline>();

            if (endOutline != null)
            {
                CabinetSlot slot = endOutline.GetComponentInParent<CabinetSlot>();
                if (slot != null && slot.GetClue() != null && firstSelectedSlot != null && slot != firstSelectedSlot)
                {
                    string clueId1 = firstSelectedSlot.GetClue().id;
                    string clueId2 = slot.GetClue().id;

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