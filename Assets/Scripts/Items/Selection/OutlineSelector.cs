using UnityEngine;
using Dialogs;

public class OutlineSelector : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float rayDistance = 100f;
    [SerializeField] private OverlayInfoManager overlayInfo;

    private Outline currentOutline;
    private DialogInteractable currentDialogInteractable;

    void Update()
    {
        // Получаем центр экрана
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        // Создаем луч из центра экрана
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        // Выполняем райкаст
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // Пытаемся получить компонент Outline
            Outline outline = hit.collider.GetComponent<Outline>();

            if (outline != null)
            {
                Select(outline);

                // Проверяем наличие DialogInteractable
                DialogInteractable dialogInteractable = hit.collider.GetComponent<DialogInteractable>();
                if (dialogInteractable == null)
                {
                    dialogInteractable = hit.collider.GetComponentInParent<DialogInteractable>();
                }

                currentDialogInteractable = dialogInteractable;

                // Обработка клика на DialogInteractable
                if (currentDialogInteractable != null && Input.GetMouseButtonDown(0))
                {
                    if (DialogManager.Instance != null && !DialogManager.Instance.IsInDialog)
                    {
                        // Используем рефлексию для вызова приватного метода TryStartDialog
                        var method = currentDialogInteractable.GetType().GetMethod("TryStartDialog",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (method != null)
                        {
                            method.Invoke(currentDialogInteractable, null);
                        }
                    }
                }

                return;
            }
        }

        // Если луч ничего не попал, отключаем обводку у предыдущего объекта
        Unselect();
        currentDialogInteractable = null;
    }

    void Select(Outline outline)
    {
        if (currentOutline != outline)
        {
            if (currentOutline != null)
            {
                currentOutline.enabled = false;
            }

            outline.enabled = true;
            currentOutline = outline;

            CabinetSlot slot = outline.GetComponentInParent<CabinetSlot>();
            if (slot != null)
            {
                overlayInfo.ShowOverlay(slot.GetClue());
            }
            else
            {
                overlayInfo.ShowOverlay(null);
            }
        }
    }

    void Unselect()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
            overlayInfo.ShowOverlay(null);
        }
    }

    public Outline GetCurrentOutline()
    {
        return currentOutline;
    }
}