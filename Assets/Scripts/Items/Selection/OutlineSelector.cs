using UnityEngine;
using UnityEngine.InputSystem;
using Dialogs;
using Tutorial;

public class OutlineSelector : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float rayDistance = 100f;
    [SerializeField] private OverlayInfoManager overlayInfo;
    [SerializeField] private InputActionReference clickAction;

    private Outline currentOutline;
    private DialogInteractable currentDialogInteractable;
    private LevelTransition currentLevelTransition;

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

                // Проверяем наличие LevelTransition
                LevelTransition levelTransition = hit.collider.GetComponent<LevelTransition>();
                if (levelTransition == null)
                {
                    levelTransition = hit.collider.GetComponentInParent<LevelTransition>();
                }

                currentLevelTransition = levelTransition;

                // Обработка клика
                if (clickAction != null && clickAction.action.WasPressedThisFrame())
                {
                    // Приоритет: сначала LevelTransition, потом DialogInteractable
                    if (currentLevelTransition != null)
                    {
                        currentLevelTransition.TransitionToLevel();
                    }
                    else if (currentDialogInteractable != null)
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
                }

                return;
            }
        }

        // Если луч ничего не попал, отключаем обводку у предыдущего объекта
        Unselect();
        currentDialogInteractable = null;
        currentLevelTransition = null;
    }

    void Select(Outline outline)
    {
        if (currentOutline != outline)
        {
            if (currentOutline != null)
            {
                // Проверяем, не является ли текущий объект активным объектом обучения
                if (!IsGuidanceObject(currentOutline))
                {
                    currentOutline.enabled = false;
                }
            }

            // Проверяем, не является ли новый объект активным объектом обучения
            if (!IsGuidanceObject(outline))
            {
                outline.enabled = true;
                currentOutline = outline;
            }
            else
            {
                // Если это объект обучения, не меняем currentOutline
                // но показываем overlay если нужно
                currentOutline = null;
            }

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
            // Проверяем, не является ли объект активным объектом обучения
            if (!IsGuidanceObject(currentOutline))
            {
                currentOutline.enabled = false;
            }
            currentOutline = null;
            overlayInfo.ShowOverlay(null);
        }
    }

    public Outline GetCurrentOutline()
    {
        return currentOutline;
    }

    /// <summary>
    /// Проверяет, является ли объект активным объектом обучения
    /// </summary>
    private bool IsGuidanceObject(Outline outline)
    {
        if (outline == null) return false;

        // Проверяем, есть ли компонент GuidanceHighlight
        GuidanceHighlight guidanceHighlight = outline.GetComponent<GuidanceHighlight>();
        if (guidanceHighlight == null)
        {
            guidanceHighlight = outline.GetComponentInParent<GuidanceHighlight>();
        }

        if (guidanceHighlight == null) return false;

        // Проверяем, активна ли система обучения и является ли этот объект текущим
        if (GuidanceManager.Instance != null && GuidanceManager.Instance.IsGuidanceActive())
        {
            return GuidanceManager.Instance.IsCurrentGuidanceObject(guidanceHighlight) && guidanceHighlight.IsHighlighted();
        }

        return false;
    }
}